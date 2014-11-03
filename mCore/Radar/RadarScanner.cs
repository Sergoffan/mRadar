using ArcheBuddy.Bot.Classes;
using mCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mCore.Radar
{
    class RadarScanner
    {
        public static uint[] NeutralFactions = new uint[] {
            162 //Neutral Guard
        };
        public static List<ClassifiedObject> ScanCreatures(RadarSettings settings, Core ArcheBuddyCore)
        {
            List<Creature> AllCreatures = ArcheBuddyCore.getCreatures();
            List<ClassifiedObject> FilteredCreatures = new List<ClassifiedObject>(AllCreatures.Count);

            foreach (Creature c in AllCreatures)
            {
                if (c.type == BotTypes.Housing && settings.HouseScanSettings.ShowRealEstate)
                {
                    Housing h = (Housing)c;
                    //this is a house, but what kind?
                    if (h.housingId == 267) //Scarecrow Garden
                    {
                        FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.ScarecrowGarden, obj = c });
                        continue;
                    }
                    else if (h.housingId == 89) //Scarecrow Farm
                    {
                        FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.ScarecrowFarm, obj = c });
                        continue;
                    }
                    else if (h.housingId == 171 || h.housingId == 199 || 
                            h.housingId == 182 || h.housingId == 200 || 
                            h.housingId == 202 || h.housingId == 204 || //actually some of these are Manors
                            h.housingId == 194 || h.housingId == 203 || 
                            h.housingId == 201 || h.housingId == 197) //ThatchedFarmhouse or otherwise 24x24
                    {
                        FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.Farmhouse, obj = c });
                        continue;
                    }
                    else if (h.housingId >= 172 && h.housingId <= 181) //Small house
                    {   
                        FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.SmallHousing, obj = c });
                        continue;
                    }
                    else if (h.housingId == 41 || h.housingId == 42 || h.housingId == 43 || h.housingId == 90)
                    {
                        FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.HousingWorkbench, obj = c });
                        continue;
                    }
                    else if (false)
                    {
                        //i don't know what 28x28 or greater houses look like yet
                        //FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.LargeHousing, obj = c });
                    }
                }

                
                if (c.type == BotTypes.Player) {

                    if (ArcheBuddyCore.isAlly(c) && settings.ActiveTab.ShowAlliedPlayers) {
                        FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.FriendlyPlayer, obj = c });
                        continue;
                    } else if (ArcheBuddyCore.isEnemy(c) && settings.ActiveTab.ShowEnemyPlayers) {
                        FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.EnemyPlayer, obj = c });
                        continue;
                    }       
                }

                if (c.type == BotTypes.Npc)
                {
                    //ignore Glorious Nui because, who cares
                    if (c.name.Equals("Glorious Nui")) continue;

                    if (c.factionId == 1054) continue; //ignore fast travel portals

                    if (ArcheBuddyCore.isAlly(c) && settings.ActiveTab.ShowFriendlyNPCs)
                    {
                        FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.FriendlyNPC, obj = c });
                        continue;
                    }
                    else if (ArcheBuddyCore.isEnemy(c) && settings.ActiveTab.ShowEnemyNPCs)
                    {
                        FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.EnemyNPC, obj = c });
                        continue;
                    }     
                }

                if (c.type == BotTypes.Slave)
                {
                    Slave s = (Slave)c;
                    if (s.slaveId == 14 || s.slaveId == 76) //clippers
                    {
                        if (ArcheBuddyCore.isAlly(s) && settings.ActiveTab.ShowAlliedPlayers)
                        {
                            FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.FriendlyClipper, obj = s });
                            continue;
                        }
                        else if (ArcheBuddyCore.isEnemy(s) && settings.ActiveTab.ShowEnemyPlayers)
                        {
                            FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.EnemyClipper, obj = s });
                            continue;
                        }
                    } else if (s.slaveId == 110) { //fishing boats

                    }
                    else if (s.slaveId == 75) //merchant ships
                    {
                        if (ArcheBuddyCore.isAlly(s) && settings.ActiveTab.ShowAlliedPlayers)
                        {
                            FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.FriendlyMerchantShip, obj = s });
                            continue;
                        }
                        else if (ArcheBuddyCore.isEnemy(s) && settings.ActiveTab.ShowEnemyPlayers)
                        {
                            FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.EnemyMerchantShip, obj = s });
                            continue;
                        }
                    }
                    else if (s.slaveId == 21) //galleons 
                    {
                        if (ArcheBuddyCore.isAlly(s) && settings.ActiveTab.ShowAlliedPlayers)
                        {
                            FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.FriendlyGalleon, obj = s });
                            continue;
                        }
                        else if (ArcheBuddyCore.isEnemy(s) && settings.ActiveTab.ShowEnemyPlayers)
                        {
                            FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.EnemyGalleon, obj = s });
                            continue;
                        }
                    }
                }
                if (c.type == BotTypes.Transfer)
                {
                    //it's an airship or carriage
                }
            }
            return FilteredCreatures;
        }
        public static List<ClassifiedObject> ScanDoodads(RadarTab settings, Core ArcheBuddyCore)
        {
            List<DoodadObject> AllDoodads = ArcheBuddyCore.getDoodads();
            List<ClassifiedObject> FilteredDoodads = new List<ClassifiedObject>(AllDoodads.Count);
            foreach (DoodadObject doodad in AllDoodads)
            {
                //ignore doodads that are in protected plant zones, or public farms
                if (doodad.plantZoneId != 0 || doodad.commonFarmId > 0) continue;

                if (doodad.name.StartsWith("Thunderstruck"))
                {
                    FilteredDoodads.Add(new ClassifiedObject() { obj = doodad, category = ObjectCategory.ThunderstruckTree });
                    continue;
                }

                if (doodad.name.StartsWith("Sunken Treasure") || doodad.name.StartsWith("Old Jar") || doodad.name.StartsWith("Old Relic"))
                {
                    FilteredDoodads.Add(new ClassifiedObject() { obj = doodad, category = ObjectCategory.Treasure });
                    continue;
                }

                if (doodad.name.Contains("Schooling")) {
                    FilteredDoodads.Add(new ClassifiedObject() { obj = doodad, category = ObjectCategory.FishSchool });
                    continue;
                }
                //try to identify the doodad using its skills
                Skill sk = GetFirstDoodadSkill(doodad);
                List<Skill> allSkills = doodad.getUseSkills();
                if (sk == null)
                {
                    //doodad is not interactable at all, or is a buried treasure site
                    continue;    
                }

                if (settings.ShowHarvestableTrees) {
                    if (allSkills.Any(s => s.name.StartsWith("Find Fruit")))
                    {
                        FilteredDoodads.Add(new ClassifiedObject() { obj = doodad, category = ObjectCategory.FruitedTree });
                        continue;
                    }
                    if (allSkills.Any(s => s.name.StartsWith("Chop Tree")))
                    {
                        FilteredDoodads.Add(new ClassifiedObject() { obj = doodad, category = ObjectCategory.HarvestableTree });
                        continue;
                    }
                }

                if (settings.ShowHarvestablePlants) {
                    if (sk.name.StartsWith("Gather") || sk.name.StartsWith("Harvest")) {
                        FilteredDoodads.Add(new ClassifiedObject() { obj = doodad, category = ObjectCategory.HarvestablePlant });
                        continue;
                    }

                    if (doodad.id == 1671) { //Both Iron and Fortuna veins seem to have this same id
                        FilteredDoodads.Add(new ClassifiedObject() { obj = doodad, category = ObjectCategory.Ore });
                        continue;
                    }

                    if (sk.name.StartsWith("Butcher") && settings.ShowHarvestablePlants)
                    {
                        FilteredDoodads.Add(new ClassifiedObject() { obj = doodad, category = ObjectCategory.Butcherable });
                        continue;
                    }
                }

                if (settings.ShowUprootable) {
                    if (sk.id == 13789 || sk.name.StartsWith("Uproot"))
                    {
                        FilteredDoodads.Add(new ClassifiedObject() { obj = doodad, category = ObjectCategory.Uprootable });
                        continue;
                    }
                }
                        
                if (settings.ShowTradePacks && TradePackNames.Any(doodad.name.Contains))
                {
                    if (sk.name.StartsWith("Collect"))
                    {
                        //trade pack could be bugged and at the bottom of the world or something.  IDK why but these show up a lot
                        if (Math.Abs(ArcheBuddyCore.me.Z - doodad.Z) < 200) 
                            FilteredDoodads.Add(new ClassifiedObject() { obj = doodad, category = ObjectCategory.TradePack });

                        continue;
                    }
                }

            }
            return FilteredDoodads;

            //ArcheBuddyCore.Log("Scanned and found " + doodads.Count.ToString() + " doodads.");
        }

        public static Skill GetFirstDoodadSkill(DoodadObject dood)
        {
            if (dood != null) return dood.getUseSkills().FirstOrDefault();
            return null;
        }

        public static string[] TradePackNames = {"Arcum Iris",
            "Falcorth",
            "Hasla",
            "Mahadevi",
            "Perinoor",
            "Rookborne",
            "Silent Forest",
            "Solis",
            "Tigerspine",
            "Villanelle",
            "Windscour",
            "Ynystere",
            "Cinderstone",
            "Dewstone",
            "Gweonid",
            "Halcyona",
            "Hellswamp",
            "Lilyut",
            "Marianople",
            "Sanddeep",
            "Solzreed",
            "Two Crowns",
            "White Arden",
            "Iron Pack",
            "Fabric Pack",
            "Marlin",
            "Sailfish",
            "Lumber Pack",};
    }
}
