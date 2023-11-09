//**********************************************************************************************************************
//  MonsterType
//  This class holds the information used to generate a pack of monsters

namespace BlazorDungeonCrawler.Server.Models
{
    public class MonsterType {
        //****************************
        //***************** Attributes
        public string Name { get; set; }                    //Display name


        public int FloorStart { get; set; }                 //Highest floor monster is seen at
        public int FloorEnd { get; set; }                   //Lowest floor monster is seen at


        public int MaxPackNumber { get; set; }              //Maximum amount to generate


        public int HealthDiceCount { get; set; }            //Amount of die used to generate health stat
        public int DamageDiceCount { get; set; }            //Amount of die used to generate damage stat
        public int ProtectionDiceCount { get; set; }        //Amount of die used to generate protection stat


        public string Documentation { get; set; }           //URL showing D&D attributes 


        //****************************
        //*************** Constructors
        public MonsterType()
        {
            Name = string.Empty;
            Documentation = string.Empty;
        }
    }
}