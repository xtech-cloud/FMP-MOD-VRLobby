
using System.Xml.Serialization;

namespace XTC.FMP.MOD.VRLobby.LIB.Unity
{
    /// <summary>
    /// 配置类
    /// </summary>
    public class MyConfig : MyConfigBase
    {
        public class BannerEntry
        {
            [XmlElement("Banner")]
            public UiElement banner { get; set; } = new UiElement();
            [XmlArray("Subjects"), XmlArrayItem("Subject")]
            public Subject[] subjects { get; set; } = new Subject[0];
        }

        public class BannerMenu
        {
            [XmlElement("Title")]
            public UiElement title { get; set; }

            [XmlAttribute("distance")]
            public float distance { get; set; } = 2;

            [XmlAttribute("space")]
            public float space { get; set; } = 20;

            [XmlArray("BannerEntries"), XmlArrayItem("BannerEntry")]
            public BannerEntry[] entries { get; set; } = new BannerEntry[0];
        }

        public class Style
        {
            [XmlAttribute("name")]
            public string name { get; set; } = "";

            [XmlAttribute("active")]
            public string active { get; set; } = "";

            [XmlElement("BannerMenu")]
            public BannerMenu bannerMenu { get; set; } = new BannerMenu();
        }


        [XmlArray("Styles"), XmlArrayItem("Style")]
        public Style[] styles { get; set; } = new Style[0];
    }
}

