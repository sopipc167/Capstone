using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using static System.Net.WebRequestMethods;
using UnityEngine.InputSystem;

public class UIInputField : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField keyword;
    [SerializeField]
    private Button search;
    [SerializeField]
    private GameObject contents;
    Dictionary<string, string> fields = new Dictionary<string, string>
    {
        {"Andromeda", "안드로메다자리"},
        {"Antlia", "공기펌프자리"},
        {"Apus", "극락조자리"},
        {"Aquarius", "물병자리"},
        {"Aquila", "독수리자리"},
        {"Ara", "제단자리"},
        {"Aries", "양자리"},
        {"Auriga", "마차부자리"},
        {"Bootes", "목동자리"},
        {"Caelum", "조각칼자리"},
        {"Camelopardalis", "기린자리"},
        {"Cancer", "게자리"},
        {"CanesVenatici", "사냥개자리"},
        {"CanisMajor", "큰개자리"},
        {"CanisMinor", "작은개자리"},
        {"Capricornus", "염소자리"},
        {"Carina", "용골자리"},
        {"Cassiopeia", "카시오페이아자리"},
        {"Centaurus", "센타우루스자리"},
        {"Cepheus", "세페우스자리"},
        {"Cetus", "고래자리"},
        {"Chamaeleon", "카멜레온자리"},
        {"Circinus", "컴퍼스자리"},
        {"Columba", "비둘기자리"},
        {"ComaBerenices", "머리털자리"},
        {"CoronaAustralis", "남쪽왕관자리"},
        {"CoronaBorealis", "북쪽왕관자리"},
        {"Corvus", "까마귀자리"},
        {"Crater", "잔자리"},
        {"Crux", "남십자자리"},
        {"Cygnus", "백조자리"},
        {"Delphinus", "돌고래자리"},
        {"Dorado", "황새치자리"},
        {"Draco", "용자리"},
        {"Equuleus", "조랑말자리"},
        {"Eridanus", "에리다누스자리"},
        {"Fornax", "화로자리"},
        {"Gemini", "쌍둥이자리"},
        {"Grus", "두루미자리"},
        {"Hercules", "헤라클레스자리"},
        {"Horologium", "시계자리"},
        {"Hydra", "바다뱀자리"},
        {"Hydrus", "작은바다뱀자리"},
        {"Indus", "인디언자리"},
        {"Lacerta", "도마뱀자리"},
        {"Leo", "사자자리"},
        {"LeoMinor", "작은사자자리"},
        {"Lepus", "토끼자리"},
        {"Libra", "천칭자리"},
        {"Lupus", "이리자리"},
        {"Lynx", "스라소니자리"},
        {"Lyra", "거문고자리"},
        {"Mensa", "테이블산자리"},
        {"Microscopium", "현미경자리"},
        {"Monoceros", "외뿔소자리"},
        {"Musca", "파리자리"},
        {"Norma", "자"},
        {"Octans", "팔분의자리"},
        {"Ophiuchus", "뱀주인자리"},
        {"Orion", "오리온자리"},
        {"Pavo", "공작자리"},
        {"Pegasus", "페가수스자리"},
        {"Perseus", "페르세우스자리"},
        {"Phoenix", "봉황자리"},
        {"Pictor", "화가자리"},
        {"Pisces", "물고기자리"},
        {"PiscisAustrinus", "남쪽물고기자리"},
        {"Puppis", "고물자리"},
        {"Pyxis", "나침반자리"},
        {"Reticulum", "그물자리"},
        {"Sagitta", "화살자리"},
        {"Sagittarius", "궁수자리"},
        {"Scorpius", "전갈자리"},
        {"Sculptor", "조각가자리"},
        {"Scutum", "방패자리"},
        {"Serpens", "뱀자리"},
        {"Sextans", "육분의자리"},
        {"Taurus", "황소자리"},
        {"Telescopium", "망원경자리"},
        {"Triangulum", "삼각형자리"},
        {"Triangulum Australe", "남쪽삼각형자리"},
        {"Tucana", "큰부리새자리"},
        {"UrsaMajor", "큰곰자리"},
        {"UrsaMinor", "작은곰자리"},
        {"Vela", "돛자리"},
        {"Virgo", "처녀자리"},
        {"Volans", "날치자리"},
        {"Vulpecula", "여우자리"},
        {"Sun","태양" },
        {"Moon","달" },
        {"Mercury","수성"},
        { "Venus","금성"},
        { "Mars","화성"},
        {"Jupiter","목성" },
        { "Saturn","토성"},
        { "Uranus","천왕성"},
        {"Neptune","해왕성" }

    };

    public void SearchButtonClick()
    {
        string text = keyword.text;
        foreach (KeyValuePair<string,string> item in fields)
        {
            if (item.Value == text)
            {
                text= item.Key;
            }
        }
        StartCoroutine(GetRequest(text));
    }
    IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get("https://port-0-capstoneserver-m2qhwewx334fe436.sel4.cloudtype.app/api/stellar/" + url)) 
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.responseCode == 200)
            {
                Debug.Log(webRequest.downloadHandler.text);
                contents.GetComponent<UIScrollView>().MakeItem(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError(webRequest.error);
            }
        }
    }
}