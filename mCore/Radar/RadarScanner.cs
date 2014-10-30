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
        //115 enemy bird
        //162 
        public static uint[] AlliedFactionsEast = new uint[] {
            1, //glorious nui, gold trader
            2, //neutral critters
            111, //warehouse manager, other npcs
            149, //blue salt brotherhood
            113, 109, //farran
            157, //stablehands
            164, //airship conductor, other critters
            168 //harani guard

        };

        public static uint[] AlliedFactionsWest = new uint[] {
            1,
            2, //neutral critter
            101, //Nuian
            103, //Elf
            167, //Nuia Alliance Sentry
        };

        public static uint[] AlliedFactionsPirate = new uint[] {
            1
        };

        public static uint[] NeutralFactions = new uint[] {
            162 //Neutral Guard
        };
        public static List<ClassifiedObject> ScanCreatures(RadarSettings settings, Core ArcheBuddyCore)
        {
            List<Creature> AllCreatures = ArcheBuddyCore.getCreatures();
            List<ClassifiedObject> FilteredCreatures = new List<ClassifiedObject>(AllCreatures.Count);

            uint myfaction = ArcheBuddyCore.me.factionId;
            bool PlayerIsEast = myfaction == 113 || myfaction == 109;
            bool PlayerIsPirate = !PlayerIsEast && myfaction == 9999;

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
                    else if (h.housingId == 171) //ThatchedFarmhouse
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
                }

                bool IsAllied = false;
                if (c.type == BotTypes.Player) {
                    
                    if (PlayerIsEast) IsAllied = AlliedFactionsEast.Contains(c.factionId);
                    else if (PlayerIsPirate) IsAllied = AlliedFactionsPirate.Contains(c.factionId);
                    else IsAllied = AlliedFactionsWest.Contains(c.factionId);

                    if (IsAllied && settings.ActiveTab.ShowAlliedPlayers) {
                        FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.FriendlyPlayer, obj = c });
                        continue;
                    } else if (!IsAllied && settings.ActiveTab.ShowEnemyPlayers) {
                        FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.EnemyPlayer, obj = c });
                        continue;
                    }       
                }

                if (c.type == BotTypes.Npc)
                {
                    //ignore Glorious Nui because, who cares
                    if (c.name.Equals("Glorious Nui")) continue;

                    if (c.factionId == 1054) continue; //ignore fast travel portals

                    if (PlayerIsEast) IsAllied = AlliedFactionsEast.Contains(c.factionId);
                    else if (PlayerIsPirate) IsAllied = AlliedFactionsPirate.Contains(c.factionId);
                    else IsAllied = AlliedFactionsWest.Contains(c.factionId);

                    if (IsAllied && settings.ActiveTab.ShowFriendlyNPCs)
                    {
                        FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.FriendlyNPC, obj = c });
                        continue;
                    }
                    else if (!IsAllied && settings.ActiveTab.ShowEnemyNPCs)
                    {
                        FilteredCreatures.Add(new ClassifiedObject { category = ObjectCategory.EnemyNPC, obj = c });
                        continue;
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
                if (doodad.name.StartsWith("Thunderstruck"))
                {
                    FilteredDoodads.Add(new ClassifiedObject() { obj = doodad, category = ObjectCategory.ThunderstruckTree });
                    continue;
                }

                //ignore doodads that are in protected plant zones, or public farms
                if (doodad.plantZoneId != 0 || doodad.commonFarmId > 0) continue;

                //try to identify the doodad using its skills
                Skill sk = GetFirstDoodadSkill(doodad);
                if (sk == null)
                {
                    //doodad is not interactable at all, or is a buried treasure site
                    continue;    
                }

                if (settings.ShowHarvestableTrees) {
                    if (sk.id == 13975 || sk.name.StartsWith("Chop Tree")) {
                        FilteredDoodads.Add(new ClassifiedObject() { obj = doodad, category = ObjectCategory.HarvestableTree });
                        continue;
                    }
                }

                if (settings.ShowHarvestablePlants) {
                    if (sk.name.StartsWith("Gather") || sk.name.StartsWith("Harvest")) {
                        FilteredDoodads.Add(new ClassifiedObject() { obj = doodad, category = ObjectCategory.HarvestablePlant });
                        continue;
                    }

                    if (sk.name.StartsWith("Mine Ore")) {
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
                    if (sk.name.StartsWith("Pick Up"))
                    {
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
            "Lumber Pack",};
    }
}
