using Microsoft.JSInterop;

using BlazorDungeonCrawler.Shared.Models;

using SharedTile = BlazorDungeonCrawler.Shared.Models.Tile;
using Microsoft.AspNetCore.Components;

namespace BlazorDungeonCrawler.Client.Pages {
    public partial class Index {
        //Page elements
        public List<string> ErrorReports { get; set; } = new();
        public List<string> InformationReports { get; set; } = new();

        public bool AdvanceDisabled { get; set; } = true;

        public int DungeonDepth { get; set; } = 0;

        public int TileRows { get; set; } = 0;
        public int TileColumns { get; set; } = 0;

        public bool MacGuffinFound { get; set; } = false;

        public string ApiVersion { get; set; } = "API V0.0.0";

        List<SharedTile> DungeonTiles { get; set; } = new();

        public List<Attribute> AdventurerExperienceStats { get; set; } = new();
        public List<Attribute> AdventurerHealthStats { get; set; } = new();
        public List<Attribute> AdventurerDamageStats { get; set; } = new();
        public List<Attribute> AdventurerProtectionStats { get; set; } = new();
        public List<Message> Messages { get; set; } = new();

        //Cookies
        bool? foundCookie = null;
        string cookieKeyId = "BlazorWebAppCookies-Id";

        Guid cookieId = Guid.Empty;

        //Dungeon
        Dungeon dungeon = new();
        Floor floor = new();
        Adventurer adventurer = new();

        public async Task WriteLog(string message) {
            await this.JS.InvokeVoidAsync("console.log", message);
        }

        //Cookies
        protected override void OnInitialized() {
            if (foundCookie == null) {
                CheckCookies();
            }
        }

        //  Retrieve
        private async void CheckCookies() {
            string response = GetCookie();
            Dictionary<string, string> cookies = ParseCookieResponse(response);

            if (cookies.ContainsKey(cookieKeyId)) {
                foundCookie = true;

                if (Guid.TryParse(cookies[cookieKeyId], out cookieId)) {
                    await WriteLog($"COOKIEID: {cookieId}");
                }
            } else {
                foundCookie = false;
            }
        }

        private string GetCookie() {
            return ((IJSInProcessRuntime)JS).Invoke<string>("eval", $"document.cookie");
        }

        private Dictionary<string, string> ParseCookieResponse(string value) {
            Dictionary<string, string> cookies = new();

            if (!string.IsNullOrEmpty(value)) {
                string[] values = value.Split(';');

                string cookieKey, cookieValue;
                foreach (string val in values) {
                    cookieKey = val.Substring(0, val.IndexOf('=')).Trim();
                    cookieValue = val.Substring(val.IndexOf('=') + 1);
                    cookies.Add(cookieKey, cookieValue);
                }
            }

            return cookies;
        }

        //Dungeon
        protected override async void OnAfterRender(bool firstRender) {
            ErrorReports = new();
            InformationReports = new();

            try {
                if (dungeon == null || dungeon.Id == Guid.Empty) {
                    try {
                        //Get Dungeon
                        ValidateDungeon(await DungeonManager.GenerateNewDungeon());

                        if (!await UpdatePageVariables()) {
                            InformationReports.Add("Dungeon values could not udpdated.");
                        }
                    } catch (Exception ex) {
                        ErrorReports.Add(ex.Message);
                    }
                }
            } catch (Exception ex) {
                ErrorReports.Add(ex.Message);
            }
        }

        public async Task<bool> SelectTile(Guid tileId) {
            ErrorReports = new();
            InformationReports = new();

            try {
                if (dungeon != null && dungeon.Id != Guid.Empty && tileId != Guid.Empty) {
                    try {
                        dungeon = await DungeonManager.SelectDungeonTile(dungeon.Id, tileId);
                    } catch (Exception ex) {
                        ErrorReports.Add(ex.Message);
                    }

                    if (!await UpdatePageVariables()) {
                        InformationReports.Add("Dungeon values could not udpdated.");
                    }

                    return true;
                } else {
                    //todo: trap error
                }
            } catch (Exception ex) {
                ErrorReports.Add(ex.Message);
            }

            return false;
        }

        private async Task AutomaticallyAdvanceDungeon() {
            try {
                AdventurerExperienceStats = new();
                AdventurerHealthStats = new();
                AdventurerDamageStats = new();
                AdventurerProtectionStats = new();

                if (dungeon != null && dungeon.Id != Guid.Empty) {
                    try {
                        ValidateDungeon(await DungeonManager.AutomaticallyAdvanceDungeon(dungeon.Id));
                    } catch (Exception ex) {
                        ErrorReports.Add(ex.Message);
                    }

                    if (!await UpdatePageVariables()) {
                        InformationReports.Add("Dungeon values could not udpdated.");
                    }
                } else {
                    InformationReports.Add("Dungeon can not be automatically advance as Dungeon has not been set");
                }
            } catch (Exception ex) {
                ErrorReports.Add(ex.Message);
            }
        }

        private void ValidateDungeon(Dungeon _dungeon) {
            try {
                //Check and assign Dungeon
                if (_dungeon == null || _dungeon.Id == Guid.Empty) { throw new ArgumentNullException("Dungon"); };
                dungeon = _dungeon;

                //Check and assign Dungeon Floor
                if (_dungeon.Floors == null || _dungeon.Floors.Count == 0) { throw new ArgumentNullException("Dungon Floors"); };
                Floor? _floor = _dungeon.Floors.Where(l => l.Depth == dungeon.Depth).FirstOrDefault();
                if (_floor == null || _floor.Id == Guid.Empty) { throw new ArgumentNullException("Dungon Floor"); };
                floor = _floor;

                if (_floor.Tiles == null || _floor.Tiles.Count == 0) { throw new ArgumentNullException("Dungon Floors"); };
                List<SharedTile>? _tiles = _floor.Tiles;
                if (_tiles == null || _tiles.Count == 0) { throw new ArgumentNullException("Dungon Floor Tiles"); };
                DungeonTiles = _tiles;

                //Check and assign Dungeon Floor
                if (_dungeon.Adventurer == null || _dungeon.Adventurer.Id == Guid.Empty) { throw new ArgumentNullException("Dungeon Adventurer"); }
                adventurer = _dungeon.Adventurer;

                //Check and assign Dungeon Messages
                if (_dungeon.Messages == null || _dungeon.Messages.Count == 0) { throw new ArgumentNullException("Dungeon Messages"); }
                Messages = _dungeon.Messages.OrderBy(m => m.Datestamp).ToList();
            } catch (Exception ex) {
                ErrorReports.Add(ex.Message);
            }
        }

        private async Task<bool> UpdatePageVariables() {
            try {
                //Advance button
                //  Enable
                AdvanceDisabled = false;

                //Dungeon
                DungeonDepth = dungeon.Depth;

                TileRows = floor.Rows;
                TileColumns = floor.Columns;

                MacGuffinFound = dungeon.MacGuffinFound;

                //Adventurer
                //  Stats
                //      XP
                AdventurerExperienceStats.Add(new Attribute() {
                    Label = "Level",
                    Value = adventurer.ExperienceLevel.ToString()
                });

                AdventurerExperienceStats.Add(new Attribute() {
                    Label = "Experience",
                    Value = $"{adventurer.Experience} / {adventurer.NextExperienceLevelCost}"
                });

                //      Health
                int auraPotion = adventurer.AuraPotion;
                int auraPotionDuration = adventurer.AuraPotionDuration;

                if (auraPotionDuration > 0) {
                    AdventurerHealthStats.Add(new Attribute() {
                        Label = "Aura potion",
                        Value = auraPotion.ToString(),
                        Duration = auraPotionDuration.ToString()
                    });

                    AdventurerHealthStats.Add(new Attribute() {
                        Label = "Base",
                        Value = adventurer.HealthBase.ToString()
                    });
                }

                AdventurerHealthStats.Add(new Attribute() {
                    Label = "Health",
                    Value = $"{adventurer.HealthBase + auraPotionDuration} / {adventurer.HealthInitial}"
                });

                //      Damage
                //          Potion
                int damagePotion = adventurer.DamagePotion;
                int damagePotionDuration = adventurer.DamagePotionDuration;
                if (damagePotionDuration > 0) {
                    AdventurerDamageStats.Add(new Attribute() {
                        Label = "Damage potion",
                        Value = damagePotion.ToString(),
                        Duration = damagePotionDuration.ToString()
                    });
                }

                //          Weapon
                int weapon = adventurer.Weapon;
                if (weapon > 0) {
                    AdventurerDamageStats.Add(new Attribute() {
                        Label = "Weapon",
                        Value = weapon.ToString()
                    });
                }

                if (damagePotionDuration > 0 || weapon > 0) {
                    AdventurerDamageStats.Add(new Attribute() {
                        Label = "Base",
                        Value = adventurer.DamageBase.ToString()
                    });
                }

                AdventurerDamageStats.Add(new Attribute() {
                    Label = "Damage",
                    Value = (adventurer.DamageBase + damagePotion + weapon).ToString()
                });

                //      Protection
                //          Potion
                int shieldPotion = adventurer.ShieldPotion;
                int shieldPotionDuration = adventurer.ShieldPotionDuration;
                if (shieldPotionDuration > 0) {
                    AdventurerProtectionStats.Add(new Attribute() {
                        Label = "Shield potion",
                        Value = shieldPotion.ToString(),
                        Duration = shieldPotionDuration.ToString()
                    });
                }

                //          Armour
                //              Helmet
                int armourHelmet = adventurer.ArmourHelmet;
                if (armourHelmet > 0) {
                    AdventurerProtectionStats.Add(new Attribute() {
                        Label = "Helmet",
                        Value = armourHelmet.ToString()
                    });
                }

                //              Breastplate
                int armourBreastplate = adventurer.ArmourBreastplate;
                if (armourBreastplate > 0) {
                    AdventurerProtectionStats.Add(new Attribute() {
                        Label = "Breastplate",
                        Value = armourBreastplate.ToString()
                    });
                }

                //              Gauntlet
                int armourGauntlet = adventurer.ArmourGauntlet;
                if (armourGauntlet > 0) {
                    AdventurerProtectionStats.Add(new Attribute() {
                        Label = "Gauntlet",
                        Value = armourGauntlet.ToString()
                    });
                }

                //              Greave
                int armourGreave = adventurer.ArmourGreave;
                if (armourGreave > 0) {
                    AdventurerProtectionStats.Add(new Attribute() {
                        Label = "Greave",
                        Value = armourGreave.ToString()
                    });
                }

                //              Boots
                int armourBoots = adventurer.ArmourBoots;
                if (armourBoots > 0) {
                    AdventurerProtectionStats.Add(new Attribute() {
                        Label = "Boots",
                        Value = armourBoots.ToString()
                    });
                }

                int protectionAdditions = shieldPotion + armourHelmet + armourBreastplate + armourGauntlet + armourGreave + armourBoots;
                if (protectionAdditions > 0) {
                    AdventurerProtectionStats.Add(new Attribute() {
                        Label = "Base",
                        Value = adventurer.ProtectionBase.ToString()
                    });
                }

                int protection = adventurer.ProtectionBase + protectionAdditions;
                if (protection > 0) {
                    AdventurerProtectionStats.Add(new Attribute() {
                        Label = "Protection",
                        Value = protection.ToString()
                    });
                }

                if (!adventurer.IsAlive) {
                    AdvanceDisabled = true;
                }

                //API version
                ApiVersion = $"API V{dungeon.ApiVersion}";

                await InvokeAsync(StateHasChanged);
            } catch (Exception) {
                throw;
            }

            return true;
        }
    }
}