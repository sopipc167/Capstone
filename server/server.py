from astroquery.simbad import Simbad
from astroquery.jplhorizons import Horizons
from astropy.coordinates import SkyCoord, EarthLocation, AltAz
from astropy.time import Time
import astropy.units as u
import asyncio
import websockets
import json

received_data = {}

# 예시 데이터 1
star_names = [
    'Tau Ceti',
    'Deneb',
    'Regulus',
    'Rigel',
    "Luyten's Star",
    'Meissa',
    'Zeta Ophiuchi',
    'Vega',
    'Betelgeuse',
    'Spica',
    'Sirius',
    'Antares',
    'Algieba',
    'Aldebaran',
    'Altair',
    'Alpha Centauri',
    'Epsilon Eridani',
]

# 예시 데이터 2
planets = [
    10,     # Sun
    199,    # Mercury
    299,    # Venus
    # 399,  # Earth
    499,    # Mars
    599,    # Jupiter
    699,    # Saturn
    799,    # Uranus
    899,    # Neptune
    999,    # Pluto

    # 301,  # Moon
    # 1,    # Ceres
    # 2,    # Pallas
    # 4,    # Vesta
    # 1P,   # Halley's Comet
    # -125544,  # International Space Station
]

# 데이터 처리 후 응답 생성
def process_data(data):
    # 데이터 처리
    return {'response': f'Received data from Unity: {data}'}

# WebSocket 서버 핸들러
async def data_server(websocket, path):
    global received_data

    # Unity에서 보낸 메시지 파싱
    init_msg = await websocket.recv()
    received_data = json.loads(init_msg)
    location = (received_data['location'][0], received_data['location'][1])

    # 데이터 생성 및 전송
    while True:
        try:
            new_data = {
                'location': received_data['location'],
                'time': Time.now().strftime('%Y-%m-%d %H:%M:%S'),
                'stars': get_star_datas(star_names, location)
            }

            json_data = json.dumps(new_data)
            await websocket.send(json_data)
            print(f'Sent data: {json_data}')

            await asyncio.sleep(60)  # 60초마다 데이터 전송
        except websockets.ConnectionClosed:
            print('Connection closed by client.')
            break

# 서버 실행 함수
async def main():
    async with websockets.serve(data_server, 'localhost', 8765):
        print('WebSocket server started on ws://localhost:8765')
        await asyncio.Future()  # 서버를 무기한 실행

# 태양계 외부 천체 검색 (SIMBAD)
def get_star_datas(star_names, obs_loc):
    simbad = Simbad()
    simbad.add_votable_fields('pmra', 'pmdec', 'plx', 'rv_value')
    result_table = simbad.query_objects(star_names)
    obs_location = EarthLocation(lat=obs_loc[0] * u.deg, lon=obs_loc[1] * u.deg, height=0 * u.m)
    obs_time = Time.now()
    star_datas = []
    i = 0

    for star_name, ra, dec, pm_ra_cosdec, pm_dec, parallax, radial_velocity in zip(result_table['MAIN_ID'], result_table['RA'], result_table['DEC'], result_table['PMRA'], result_table['PMDEC'], result_table['PLX_VALUE'], result_table['RV_VALUE']):

    # SkyCoord 객체로 변환
        star_coord = SkyCoord(
            ra=ra,
            dec=dec,
            unit=(u.hourangle, u.deg),
            frame='icrs',
            pm_ra_cosdec=pm_ra_cosdec * u.mas/u.yr,
            pm_dec=pm_dec * u.mas/u.yr,
            distance=1000/parallax * u.parsec,
            radial_velocity=radial_velocity * u.km/u.s,
            obstime=Time('2000-01-01T00:00:00')
        )

        star_now = star_coord.apply_space_motion(new_obstime=obs_time)
        altaz_frame = AltAz(obstime=obs_time, location=obs_location)
        star_altaz = star_now.transform_to(altaz_frame)

        star_data = {
            'name': star_names[i],
            'id': star_name,
            'ra': star_now.ra.degree,
            'dec': star_now.dec.degree,
            'alt': star_altaz.alt.degree,
            'az': star_altaz.az.degree,
        }

        star_datas.append(star_data)
        i += 1

    return star_datas

# 태양계 외부 단일 천체 검색 (SIMBAD) --- 테스트용
def get_star_data(star_name):
    # 별의 위치 정보와 고유 운동 데이터 가져오기
    simbad = Simbad()
    simbad.add_votable_fields('pmra', 'pmdec', 'plx', 'rv_value')
    result_table = simbad.query_object("Betelgeuse")

    # 정보 추출
    ra = result_table['RA'][0]
    dec = result_table['DEC'][0]
    pm_ra_cosdec = result_table['PMRA'][0]
    pm_dec = result_table['PMDEC'][0]
    parallax = result_table['PLX_VALUE'][0]
    radial_velocity = result_table['RV_VALUE'][0]

    # SkyCoord 객체로 변환
    star_coord = SkyCoord(
        ra=ra,
        dec=dec,
        unit=(u.hourangle, u.deg),
        frame='icrs',
        pm_ra_cosdec=pm_ra_cosdec * u.mas/u.yr,
        pm_dec=pm_dec * u.mas/u.yr,
        distance=1000/parallax * u.parsec,
        radial_velocity=radial_velocity * u.km/u.s,
        obstime=Time('2000-01-01T00:00:00')
    )

    # 관측 위치 설정
    obs_location = EarthLocation(lat=37.5665 * u.deg, lon=126.9780 * u.deg, height=0 * u.m)

    # 관측 시간 설정
    obs_time = Time("2024-10-05T15:00:00")

    # 고유 운동을 반영하여 위치 계산
    star_now = star_coord.apply_space_motion(new_obstime=obs_time)

    # 지평 좌표계로 변환
    altaz_frame = AltAz(obstime=obs_time, location=obs_location)
    star_altaz = star_now.transform_to(altaz_frame)

    # 결과 출력
    print(f"관측 위치: (위도: {obs_location.lat:.2f}, 경도: {obs_location.lon:.2f})")
    print(f"관측 시간: {obs_time.iso} UTC")
    print(f"적경 (RA): {star_now.ra:.6f}")
    print(f"적위 (Dec): {star_now.dec:.6f}")
    print(f"고도: {star_altaz.alt:.2f}")
    print(f"방위각: {star_altaz.az:.2f}")

    star_data = {
        'name': star_name,
        'ra': star_now.ra,
        'dec': star_now.dec,
        'alt': star_altaz.alt,
        'az': star_altaz.az,
    }

# 태양계 내부 천체 검색 (JPL HORIZONS) --- 테스트용
def get_planet_data(planet_id, obs_loc):
    obs_location = {'lon': obs_loc[1] * u.deg,  # 관측 위치 설정
                     'lat': obs_loc[0] * u.deg,
                     'elevation': 0 * u.m}
    # obs_time = Time.now()                     # 관측 시간 설정
    obs_time = Time("2024-10-08 9:00:00")

    # 천체 검색
    planet = Horizons(id=planet_id, location=obs_location, epochs=obs_time.jd)
    planet_ephem = planet.ephemerides()

    # 정보 추출
    target_name = planet_ephem['targetname'][0]
    ra = planet_ephem['RA'][0]      # 적경 [deg]
    dec = planet_ephem['DEC'][0]    # 적위 [deg]
    alt = planet_ephem['EL'][0]     # 고도 [deg]
    az = planet_ephem['AZ'][0]      # 방위각 [deg]
    v = planet_ephem['V'][0]        # 겉보기 등급

    print(f'TIME {obs_time}')
    print(f'TARGET {target_name}')
    print(f'RA {ra}, DEC {dec}, ALT {alt}, AZ {az}')
    print(f'V {v}')

if __name__ == '__main__':
    # 서버 실행
    asyncio.run(main())

    # 예시 코드
    # get_star_datas(star_names, (37.5665, 126.9780))
    # get_planet_data("299", (37.5665, 126.9780))