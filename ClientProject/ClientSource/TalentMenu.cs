using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Barotrauma;
using Barotrauma.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using MonoGame.Utilities;
using static Barotrauma.TalentTree;
using static Barotrauma.TalentTree.TalentStages;
using static CrossClass.CrossClassHelpers;
using static Barotrauma.TalentTreeStyle;

namespace CrossClass
{
    public class TalentSelectionBar
    {
        public void Create(GUILayoutGroup parent, CharacterInfo characterInfo, GUIFrame talentMenuParentFrame, TalentMenu talentMenu)
        {
            var character = characterInfo.Character;
            var availableCrossClassPoints = GetAvailableCrossClassPoints(character);
            var crossClassButtonSuffix = availableCrossClassPoints > 0 ? $" ({availableCrossClassPoints})" : "";
            var primaryTalentTree = GetPrimaryTalentTree(character);
            var selectedTalentTree = GetSelectedTalentTree(character);
            var canCrossClass = CanCrossClass(character) && !HasCrossClassTalentTree(selectedTalentTree, character) && selectedTalentTree != primaryTalentTree;
            // LuaCsLogger.LogError($"Stepped in again; available points is: {availableCrossClassPoints}");

            var crossClassLayout = new GUILayoutGroup(new RectTransform(new Vector2(1f, 0.1f), parent.RectTransform), true, Anchor.CenterLeft);
            var crossClassButton = new GUIButton(new RectTransform(new Vector2(0.2f, 0.9f), crossClassLayout.rectTransform))
            {
                OnButtonDown = () =>
                {
                    // LuaCsLogger.LogMessage("Pressed Cross Class");
                    // f.Visible = !f.Visible;
                    if (GetSelectedTalentTree(character) != GetPrimaryTalentTree(character) && CanCrossClass(character))
                    {
                        // LuaCsLogger.LogMessage($"Adding {selectedTalentTree!.Identifier} as a cross class tree.");
                        UnlockCrossClassTalentTree(GetSelectedTalentTree(character), character);
                        // CrossClassSync.Instance.UpdateConfig();
                        // CrossClassUtils.RefreshCrossClassTalentPoints(characterInfo);

                        // shouldClearCrossClassValues = false;

                        // ClearGUI(parentCached!, characterInfo, talentMenu);
                        // ResetGUI(parentCached!);
                        talentMenu.CreateGUI(talentMenuParentFrame, characterInfo);
                        return true;
                    }
                    return true;
                },
                Text = $"Cross Class{crossClassButtonSuffix}",
                Color = canCrossClass ? Color.Goldenrod : Color.Gray,
                Enabled = canCrossClass,
                TextColor = Color.White,
                HoverColor = Color.LightGoldenrodYellow,
                SelectedColor = Color.Goldenrod,
                DisabledColor = Color.DarkGoldenrod,
                PressedColor = Color.PaleGoldenrod,
                ToolTip = "Upon clicking this button when you are able to cross class, the currently selected tree will be added to your cross class list."
            };

            var crossClassButtonList = new GUIListBox(new RectTransform(new Vector2(0.8f, 0.9f), crossClassLayout.RectTransform), true);

            bool selectedIsPrimary = selectedTalentTree == primaryTalentTree;
            bool selectedIsUnlocked = HasCrossClassTalentTree(selectedTalentTree, character) || selectedIsPrimary;

            var selectedTalentLocalizedName = JobPrefab.Prefabs[selectedTalentTree.Identifier].Name;
            string selectedClassLabelText = string.Empty;

            if (selectedIsPrimary)
            {
                selectedClassLabelText = $"Selected: {TextManager.Capitalize(selectedTalentLocalizedName ?? selectedTalentTree.Identifier.ToString())} (Unlocked - Core Talent Tree)";
            }
            else if (selectedIsUnlocked)
            {
                selectedClassLabelText = $"Selected: {TextManager.Capitalize(selectedTalentLocalizedName ?? selectedTalentTree.Identifier.ToString())} (Unlocked - Cross Class)";
            }
            else
            {
                selectedClassLabelText = $"Selected: {TextManager.Capitalize(selectedTalentLocalizedName ?? selectedTalentTree.Identifier.ToString())} (Locked)";
            }

            // LuaCsLogger.LogMessage($"Menu text should be:{{\n{selectedClassLabelText}\n}}");
            var selectedClassLabel = new GUITextBlock(new RectTransform(new Vector2(1f, 0.025f), parent.RectTransform), selectedClassLabelText, font: GUIStyle.SubHeadingFont, textAlignment: Alignment.CenterLeft, color: Color.White);

            // IEnumerable<JobPrefab> jobPrefabList = Enumerable.Empty<JobPrefab>();
            // jobPrefabList = JobPrefab.Prefabs.Where((job) => job.Identifier).Prepend(primaryTalentTree!);

            // JobTalentTrees.ForEach((t)=>{

            // });

            IEnumerable<JobPrefab> jobPrefabs = JobPrefab.Prefabs.Where(
                jp => (jp.Identifier != characterInfo.Job.Prefab.Identifier) && !jp.HiddenJob
            );

            jobPrefabs = jobPrefabs.Prepend(characterInfo.Job.Prefab);

            jobPrefabs.ForEach(jobPrefab =>
            {
                GUIButton iconButton;

                // if (jobPrefab.Identifier == characterInfo.Job.Prefab.Identifier)
                // {
                //     iconButton = new GUIButton(new RectTransform(new Vector2(0.065f, 0.9f), crossClassButtonList.Content.RectTransform, Anchor.CenterLeft))
                //     {
                //         GlowOnSelect = true,
                //         OutlineColor = Color.Pink,
                //         Color = Color.Green,
                //         DisabledColor = Color.Gray,
                //         HoverColor = Color.WhiteSmoke
                //     };
                // }
                // else
                // {
                // var isCrossClassSkill = characterInfo.GetSavedStatValue(StatTypes.None, $"cross_class.{characterInfo.Job.Prefab.Identifier}");

                var primaryTalentTree = GetPrimaryTalentTree(character);
                var talentTreeForJob = GetTalentTreeForJobIdentifier(jobPrefab.Identifier);
                bool isPrimary = primaryTalentTree == talentTreeForJob;
                bool isSelectedTree = GetSelectedTalentTree(character) == talentTreeForJob;
                bool hasCrossClassTree = HasCrossClassTalentTree(talentTreeForJob, character);

                // LuaCsLogger.LogError($"State of hasCrossClassTree: #{hasCrossClassTree}");

                Color selectedColor = Color.LightGreen;
                Color selectedLockedColor = Color.LightGreen;
                Color unlockedColor = Color.White;
                Color lockedColor = Color.Gray;

                Color buttonColor = Color.Magenta;

                if (isPrimary && !hasCrossClassTree && isSelectedTree)
                {
                    buttonColor = selectedColor;
                }
                else if (isPrimary && !hasCrossClassTree && !isSelectedTree)
                {
                    buttonColor = unlockedColor;
                }
                else if (!isPrimary && hasCrossClassTree && isSelectedTree)
                {
                    buttonColor = selectedColor;
                }
                else if (!isPrimary && hasCrossClassTree && !isSelectedTree)
                {
                    buttonColor = unlockedColor;
                }
                else if (!isPrimary && !hasCrossClassTree && isSelectedTree)
                {
                    buttonColor = selectedLockedColor;
                }
                else if (!isPrimary && !hasCrossClassTree && !isSelectedTree)
                {
                    buttonColor = lockedColor;
                }

                iconButton = new GUIButton(new RectTransform(new Vector2(0.065f, 0.9f), crossClassButtonList.Content.RectTransform, Anchor.CenterLeft))
                {
                    // GlowOnSelect = true,
                    Color = buttonColor,
                    // DisabledColor = Color.Gray,
                    // HoverColor = Color.WhiteSmoke,
                    ToolTip = $"{jobPrefab.Name}"
                };

                // }
                //RichString.Rich($"‖color:{Color.White.ToStringHex()}‖{jobPrefab.Name.cachedValue}‖color:end‖");

                var iconComponent = new GUIImage(
                    new RectTransform(new Vector2(1f, 1f), iconButton.RectTransform, Anchor.CenterLeft), jobPrefab.Icon, true)
                {
                    // Enabled = false,
                    Color = (hasCrossClassTree || isPrimary) ? jobPrefab.UIColor : buttonColor,
                    // DisabledColor = Color.Gray,
                    // HoverColor = Color.WhiteSmoke,
                    // ToolTip = TextManager.Get(jobPrefab.Identifier)
                    // SelectedColor = Color.White,
                };

                iconButton.OnButtonDown += (GUIButton.OnButtonDownHandler)(() =>
                {
                    SetSelectedTalentTree(jobPrefab.Identifier, character);
                    // CreateGUIForSelectedTalentTree(talentMenuParentFrame, characterInfo, talentMenu);

                    // LuaCsLogger.LogMessage($"Selected: {jobPrefab.Identifier}");

                    // if (TalentTree.JobTalentTrees.TryGet(jobPrefab.Identifier, out TalentTree? selectedJob))
                    // {
                    // talentMenu.CreateTalentMenu(contentLayout!, characterInfo, selectedJob!);
                    // talentMenu.CreateFooter(contentLayout!, characterInfo);
                    // talentMenu.UpdateTalentInfo();
                    // }

                    talentMenu.CreateGUI(talentMenuParentFrame, characterInfo);
                    return true;
                });

                // iconComponent.OnDraw += (Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, GUICustomComponent c) => {
                // sprite.Draw(spriteBatch, crossClassLayout.Rect.Center.ToVector2(), Color.Orange, scale: Math.Min(crossClassLayout.Rect.Width / (float)sprite.SourceRect.Width, crossClassLayout.Rect.Height / (float)sprite.SourceRect.Height));
                // };
            });

            // crossClassButtonList.RectTransform.MinSize = new Point(crossClassButtonList.Rect.Width + (int)(crossClassButtonList.ScrollBar.Rect.Width * 0.9f), crossClassButtonList.Content.Rect.Height);
        }
    }

    [HarmonyPatch]
    public class TalentMenu_Patches
    {
        // static GUIFrame? parentCached = null;
        static GUIListBox? mainList = null;
        static GUIFrame? content = null;
        static GUILayoutGroup? contentLayout = null;
        static GUILayoutGroup? subTreeLayoutGroup = null;
        static GUIListBox? specializationList = null;
        static GUILayoutGroup? crossClassLayout = null;
        static GUIListBox? crossClassButtonList = null;
        static GUILayoutGroup? footerMainLayout = null;
        static GUIButton? crossClassButton = null;
        static GUITextBlock? selectedClassLabel = null;
        static GUIFrame? background;
        static GUIFrame? horizontalLine;
        static int padding;
        static GUIFrame? frame;
        static TalentSelectionBar talentSelectionBar = new TalentSelectionBar();
        
         private static readonly Color unselectedColor = new Color(240, 255, 255, 225),
                                      unselectableColor = new Color(100, 100, 100, 225),
                                      pressedColor = new Color(60, 60, 60, 225),
                                      lockedColor = new Color(48, 48, 48, 255),
                                      unlockedColor = new Color(24, 37, 31, 255),
                                      availableColor = new Color(50, 47, 33, 255);

        private static readonly ImmutableDictionary<TalentStages, TalentTreeStyle> talentStageStyles =
            new Dictionary<TalentStages, TalentTreeStyle>
            {
                [Invalid] = new TalentTreeStyle("TalentTreeLocked", lockedColor),
                [Locked] = new TalentTreeStyle("TalentTreeLocked", lockedColor),
                [Unlocked] = new TalentTreeStyle("TalentTreePurchased", unlockedColor),
                [Available] = new TalentTreeStyle("TalentTreeUnlocked", availableColor),
                [Highlighted] = new TalentTreeStyle("TalentTreeAvailable", availableColor)
            }.ToImmutableDictionary();


        // static void CreateGUIForSelectedTalentTree(GUIFrame parent, CharacterInfo? characterInfo, TalentMenu __instance)
        // {
        //     LuaCsLogger.Log("Talent Menu create GUI fully overridden");

        //     ClearGUI(parent, characterInfo, __instance);
        //     ResetGUI(parent);

        //     if (characterInfo is null) { return; }

        //     __instance.CreateStatPanel(contentLayout!, characterInfo);
        //     horizontalLine = new GUIFrame(new RectTransform(new Vector2(1f, 1f), contentLayout!.RectTransform), style: "HorizontalLine");

        //     // if (TalentTree.JobTalentTrees.TryGet(characterInfo.Job.Prefab.Identifier, out TalentTree? talentTree))
        //     // {
        //     // selectedTalentTree ??= GetDefaultTalentTree(characterInfo.Job.Prefab.Identifier);

        //     CreateTalentSelectionBar(contentLayout, characterInfo, parent, __instance);
        //     __instance.CreateTalentMenu(contentLayout, characterInfo, selectedTalentTree!);

        //     // }

        //     __instance.CreateFooter(contentLayout, characterInfo);
        //     __instance.UpdateTalentInfo();

        //     if (GameMain.NetworkMember != null && TalentMenu.IsOwnCharacter(characterInfo))
        //     {
        //         __instance.CreateMultiplayerCharacterSettings(frame!, content!);
        //     }
        // }

        static GUIFrame? talentMenuParent;

        [HarmonyPrefix]
        [HarmonyPatch("Barotrauma.TalentMenu", "CreateGUI")]
        static bool CreateGUI(GUIFrame parent, CharacterInfo? characterInfo, TalentMenu __instance)
        {
            // if(CrossClassSync.Instance.Initialized == false)
            //     return true;
            
            talentMenuParent ??= parent;
            // if(!CrossClassSync.Instance.Initialized) return true;
            // CrossClassSync.Instance.UpdateConfig();
            
            __instance.characterInfo = characterInfo;
            __instance.character = characterInfo?.Character;

            parent.ClearChildren();
            __instance.talentButtons.Clear();
            __instance.talentShowCaseButtons.Clear();
            __instance.talentCornerIcons.Clear();
            __instance.showCaseTalentFrames.Clear();

            background = new GUIFrame(new RectTransform(Vector2.One, parent.RectTransform, Anchor.TopCenter), style: "GUIFrameListBox");
            padding = GUI.IntScale(15);
            frame = new GUIFrame(new RectTransform(new Point(background.Rect.Width - padding, background.Rect.Height - padding), parent.RectTransform, Anchor.Center), style: null);

            content = new GUIFrame(new RectTransform(new Vector2(0.98f), frame.RectTransform, Anchor.Center), style: null);
            contentLayout = new GUILayoutGroup(new RectTransform(Vector2.One, content.RectTransform, anchor: Anchor.Center), childAnchor: Anchor.TopCenter)
            {
                AbsoluteSpacing = GUI.IntScale(10),
                Stretch = true
            };

            if (characterInfo != null && characterInfo?.Character != null)
            {
                var character = characterInfo.Character;
                // foreach(var talent in characterInfo.UnlockedTalents)
                // {
                //     LuaCsLogger.Log($"Character {characterInfo.Name} has `{talent}");
                // }

                // if(TalentMenu.IsOwnCharacter(characterInfo))
                // {
                UpdateCharacterTotalTalentPoints(character);
                // if(GameMain.IsSingleplayer)
                // {
                //     CrossClassSync.Instance.SaveCharacter(characterInfo);
                // }

                // if(string.IsNullOrEmpty(CrossClassSync.Instance.CharacterConfig.CharacterData.PrimaryClass))
                // {
                //     SetPrimaryTalentTree(characterInfo.Job.Prefab.Identifier.ToString());
                // }
                
                // if(string.IsNullOrEmpty(CrossClassSync.Instance.CharacterConfig.CharacterData.SelectedClass))
                // {
                //     SetSelectedTalentTree(characterInfo.Job.Prefab.Identifier.ToString());
                // }
                __instance.CreateStatPanel(contentLayout!, characterInfo);
                horizontalLine = new GUIFrame(new RectTransform(new Vector2(1f, 1f), contentLayout!.RectTransform), style: "HorizontalLine");
                talentSelectionBar = new TalentSelectionBar();

                talentSelectionBar.Create(contentLayout, characterInfo, parent, __instance);
                __instance.CreateTalentMenu(contentLayout, characterInfo, GetSelectedTalentTree(character));
                // }
                // else
                // {
                //     __instance.CreateStatPanel(contentLayout!, characterInfo);
                //     if(GetTalentTreeForJobIdentifier(characterInfo.Job.Prefab.Identifier) is TalentTree jobTree)
                //     {
                //         horizontalLine = new GUIFrame(new RectTransform(new Vector2(1f, 1f), contentLayout!.RectTransform), style: "HorizontalLine");
                //         __instance.CreateTalentMenu(contentLayout, characterInfo, jobTree);
                //     }                    
                // }

                __instance.CreateFooter(contentLayout, characterInfo);
                __instance.UpdateTalentInfo();

                if (GameMain.NetworkMember != null && TalentMenu.IsOwnCharacter(characterInfo))
                {
                    __instance.CreateMultiplayerCharacterSettings(frame!, content!);
                }
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Barotrauma.TalentMenu", "CreateTalentMenu")]
        static bool CreateTalentMenu(GUIComponent parent, CharacterInfo info, TalentTree tree, TalentMenu __instance)
        {
            __instance.talentMainArea = new GUIFrame(new RectTransform(new Vector2(1f, 0.9f), parent.RectTransform, Anchor.TopCenter), style: null);

            mainList = new GUIListBox(new RectTransform(Vector2.One, __instance.talentMainArea.RectTransform));
            __instance.startAnimation = TalentMenu.CreatePopupAnimationHandler(__instance.talentMainArea);

            if (info is { TalentRefundPoints: > 0, ShowTalentResetPopupOnOpen: true })
            {
                __instance.CreateTalentResetPopup(__instance.talentMainArea);
            }

            __instance.selectedTalents = info.GetUnlockedTalentsInTree().ToHashSet();

            var specializationCount = tree.TalentSubTrees.Count(t => t.Type == TalentTreeType.Specialization);

            List<GUITextBlock> subTreeNames = new List<GUITextBlock>();
            foreach (var subTree in tree.TalentSubTrees)
            {
                GUIListBox talentList;
                GUIComponent talentParent;
                Vector2 treeSize;
                switch (subTree.Type)
                {
                    case TalentTreeType.Primary:
                        talentList = mainList;
                        treeSize = new Vector2(1f, 0.5f);
                        break;
                    case TalentTreeType.Specialization:
                        talentList = GetSpecializationList();
                        treeSize = new Vector2(Math.Max(0.333f, 1.0f / tree.TalentSubTrees.Count(t => t.Type == TalentTreeType.Specialization)), 1f);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Invalid TalentTreeType \"{subTree.Type}\"");
                }
                talentParent = talentList.Content;

                subTreeLayoutGroup = new GUILayoutGroup(new RectTransform(treeSize, talentParent.RectTransform), isHorizontal: false, childAnchor: Anchor.TopCenter)
                {
                    Stretch = true
                };

                if (subTree.Type != TalentTreeType.Primary)
                {
                    GUIFrame subtreeTitleFrame = new GUIFrame(new RectTransform(new Vector2(1f, 0.05f), subTreeLayoutGroup.RectTransform, anchor: Anchor.TopCenter)
                    { MinSize = new Point(0, GUI.IntScale(30)) }, style: null);
                    subtreeTitleFrame.RectTransform.IsFixedSize = true;
                    int elementPadding = GUI.IntScale(8);
                    Point headerSize = subtreeTitleFrame.RectTransform.NonScaledSize;
                    GUIFrame subTreeTitleBackground = new GUIFrame(new RectTransform(new Point(headerSize.X - elementPadding, headerSize.Y), subtreeTitleFrame.RectTransform, anchor: Anchor.Center), style: "SubtreeHeader");
                    subTreeNames.Add(new GUITextBlock(new RectTransform(Vector2.One, subTreeTitleBackground.RectTransform, anchor: Anchor.TopCenter), subTree.DisplayName, font: GUIStyle.SubHeadingFont, textAlignment: Alignment.Center));
                }

                int optionAmount = subTree.TalentOptionStages.Length;
                for (int i = 0; i < optionAmount; i++)
                {
                    TalentOption option = subTree.TalentOptionStages[i];
                    __instance.CreateTalentOption(subTreeLayoutGroup, subTree, i, option, info, specializationCount);
                }
                subTreeLayoutGroup.RectTransform.Resize(new Point(subTreeLayoutGroup.Rect.Width,
                    subTreeLayoutGroup.Children.Sum(c => c.Rect.Height + subTreeLayoutGroup.AbsoluteSpacing)));
                subTreeLayoutGroup.RectTransform.MinSize = new Point(subTreeLayoutGroup.Rect.Width, subTreeLayoutGroup.Rect.Height);
                subTreeLayoutGroup.Recalculate();

                if (subTree.Type == TalentTreeType.Specialization)
                {
                    talentList.RectTransform.Resize(new Point(talentList.Rect.Width, Math.Max(subTreeLayoutGroup.Rect.Height, talentList.Rect.Height)));
                    talentList.RectTransform.MinSize = new Point(0, talentList.Rect.Height);
                }
            }

            specializationList = GetSpecializationList();
            //resize (scale up) children if there's less than 3 of them to make them cover the whole width of the menu
            specializationList.Content.RectTransform.Resize(new Point(specializationList.Content.Children.Sum(static c => c.Rect.Width), specializationList.Rect.Height),
                resizeChildren: specializationCount < 3);
            //make room for scrollbar if there's more than the default amount of specializations
            if (specializationCount > 3)
            {
                specializationList.RectTransform.MinSize = new Point(specializationList.Rect.Width, specializationList.Content.Rect.Height + (int)(specializationList.ScrollBar.Rect.Height * 0.9f));
            }

            GUITextBlock.AutoScaleAndNormalize(subTreeNames);

            GUIListBox GetSpecializationList()
            {
                if (mainList!.Content.Children.LastOrDefault() is GUIListBox specList)
                {
                    return specList;
                }
                GUIListBox newSpecializationList = new GUIListBox(new RectTransform(new Vector2(1.0f, 0.5f), mainList.Content.RectTransform, Anchor.TopCenter), isHorizontal: true, style: null);
                return newSpecializationList;
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Barotrauma.TalentMenu", "CreateFooter")]
        static bool CreateFooter(GUIComponent parent, CharacterInfo info, TalentMenu __instance)
        {
            if (footerMainLayout != null)
            {
                footerMainLayout.ClearChildren();
                parent.RemoveChild(footerMainLayout);
            }

            footerMainLayout = new GUILayoutGroup(new RectTransform(new Vector2(1f, 0.07f), parent.RectTransform, Anchor.TopCenter), isHorizontal: true)
            {
                RelativeSpacing = 0.01f,
                Stretch = true
            };
            GUILayoutGroup gUILayoutGroup2 = new GUILayoutGroup(new RectTransform(new Vector2(0.59f, 1f), footerMainLayout.RectTransform));
            GUIFrame gUIFrame = new GUIFrame(new RectTransform(new Vector2(1f, 0.5f), gUILayoutGroup2.RectTransform), null);
            __instance.experienceBar = new GUIProgressBar(new RectTransform(new Vector2(1f, 1f), gUIFrame.RectTransform, Anchor.CenterLeft), info.GetProgressTowardsNextLevel(), GUIStyle.Green)
            {
                IsHorizontal = true
            };
            RectTransform rectT = new RectTransform(new Vector2(1f, 1f), gUIFrame.RectTransform, Anchor.Center);
            RichString text = "";
            GUIFont font = GUIStyle.Font;
            __instance.experienceText = new GUITextBlock(rectT, text, null, font, Alignment.CenterRight)
            {
                Shadow = true,
                ToolTip = TextManager.Get("experiencetooltip")
            };
            RectTransform rectT2 = new RectTransform(new Vector2(1f, 0.5f), gUILayoutGroup2.RectTransform, Anchor.Center);
            RichString text2 = "";
            font = GUIStyle.SubHeadingFont;
            __instance.talentPointText = new GUITextBlock(rectT2, text2, null, font, Alignment.CenterRight)
            {
                AutoScaleVertical = true
            };
            __instance.talentResetButton = new GUIButton(new RectTransform(new Vector2(0.19f, 1f), footerMainLayout.RectTransform), TextManager.Get("reset"), Alignment.Center, "GUIButtonFreeScale")
            {
                OnClicked = __instance.ResetTalentSelection
            };
            __instance.talentApplyButton = new GUIButton(new RectTransform(new Vector2(0.19f, 1f), footerMainLayout.RectTransform), TextManager.Get("applysettingsbutton"), Alignment.Center, "GUIButtonFreeScale")
            {
                OnClicked = __instance.ApplyTalentSelection
            };
            GUITextBlock.AutoScaleAndNormalize(__instance.talentResetButton.TextBlock, __instance.talentApplyButton.TextBlock);
            return false;
        }

        // [HarmonyPostfix]
        // [HarmonyPatch("Barotrauma.TalentMenu", "ApplyTalents")]
        // static void ApplyTalents(Character controlledCharacter, ref TalentMenu __instance)
        // {
            // __instance.CreateGUI(talentMenuParent, __instance.characterInfo);
        // }

        [HarmonyPrefix]
        [HarmonyPatch("Barotrauma.TalentMenu", "CreateTalentOption")]
        static bool CreateTalentOption(GUIComponent parent, TalentSubTree subTree, int index, TalentOption talentOption, CharacterInfo info, int specializationCount, TalentMenu __instance, ref ImmutableDictionary<TalentStages, TalentTreeStyle> ___talentStageStyles, ref Color ___pressedColor, ref Color ___unselectableColor)
        {
            int elementPadding = GUI.IntScale(8);
            int height = GUI.IntScale((GameMain.GameSession?.Campaign == null ? 65 : 60) * (specializationCount > 3 ? 0.97f : 1.0f));
            GUIFrame talentOptionFrame = new GUIFrame(new RectTransform(new Vector2(1f, 0.01f), parent.RectTransform, anchor: Anchor.TopCenter)
            { MinSize = new Point(0, height) }, style: null);

            Point talentFrameSize = talentOptionFrame.RectTransform.NonScaledSize;

            GUIFrame talentBackground = new GUIFrame(new RectTransform(new Point(talentFrameSize.X - elementPadding, talentFrameSize.Y - elementPadding), talentOptionFrame.RectTransform, anchor: Anchor.Center),
                style: "TalentBackground")
            {
                Color = ___talentStageStyles[Locked].Color
            };
            GUIFrame talentBackgroundHighlight = new GUIFrame(new RectTransform(Vector2.One, talentBackground.RectTransform, anchor: Anchor.Center), style: "TalentBackgroundGlow") { Visible = false };

            GUIImage cornerIcon = new GUIImage(new RectTransform(new Vector2(0.2f), talentOptionFrame.RectTransform, anchor: Anchor.BottomRight, scaleBasis: ScaleBasis.BothHeight) { MaxSize = new Point(16) }, style: null)
            {
                CanBeFocused = false,
                Color = ___talentStageStyles[Locked].Color
            };

            Point iconSize = cornerIcon.RectTransform.NonScaledSize;
            cornerIcon.RectTransform.AbsoluteOffset = new Point(iconSize.X / 2, iconSize.Y / 2);

            GUILayoutGroup talentOptionCenterGroup = new GUILayoutGroup(new RectTransform(new Vector2(0.6f, 0.9f), talentOptionFrame.RectTransform, Anchor.Center), childAnchor: Anchor.CenterLeft);
            GUILayoutGroup talentOptionLayoutGroup = new GUILayoutGroup(new RectTransform(Vector2.One, talentOptionCenterGroup.RectTransform), isHorizontal: true, childAnchor: Anchor.CenterLeft) { Stretch = true };

            HashSet<Identifier> talentOptionIdentifiers = talentOption.TalentIdentifiers.OrderBy(static t => t).ToHashSet();
            HashSet<TalentButton> buttonsToAdd = new();

            Dictionary<GUILayoutGroup, ImmutableHashSet<Identifier>> showCaseTalentParents = new();
            Dictionary<Identifier, GUIComponent> showCaseTalentButtonsToAdd = new();

            foreach (var (showCaseTalentIdentifier, talents) in talentOption.ShowCaseTalents)
            {
                talentOptionIdentifiers.Add(showCaseTalentIdentifier);
                Point parentSize = talentBackground.RectTransform.NonScaledSize;
                GUIFrame showCaseFrame = new GUIFrame(new RectTransform(new Point((int)(parentSize.X / 3f * (talents.Count - 1)), parentSize.Y)), style: "GUITooltip")
                {
                    UserData = showCaseTalentIdentifier,
                    IgnoreLayoutGroups = true,
                    Visible = false
                };
                GUILayoutGroup showcaseCenterGroup = new GUILayoutGroup(new RectTransform(new Vector2(0.9f, 0.7f), showCaseFrame.RectTransform, Anchor.Center), childAnchor: Anchor.CenterLeft);
                GUILayoutGroup showcaseLayout = new GUILayoutGroup(new RectTransform(Vector2.One, showcaseCenterGroup.RectTransform), isHorizontal: true) { Stretch = true };
                showCaseTalentParents.Add(showcaseLayout, talents);
                __instance.showCaseTalentFrames.Add(showCaseFrame);
            }

            foreach (Identifier talentId in talentOptionIdentifiers)
            {
                if (!TalentPrefab.TalentPrefabs.TryGet(talentId, out TalentPrefab? talent)) { continue; }

                bool isShowCaseTalent = talentOption.ShowCaseTalents.ContainsKey(talentId);
                GUIComponent talentParent = talentOptionLayoutGroup;

                foreach (var (key, value) in showCaseTalentParents)
                {
                    if (value.Contains(talentId))
                    {
                        talentParent = key;
                        break;
                    }
                }

                GUIFrame talentFrame = new GUIFrame(new RectTransform(Vector2.One, talentParent.RectTransform), style: null)
                {
                    CanBeFocused = false
                };

                GUIFrame croppedTalentFrame = new GUIFrame(new RectTransform(Vector2.One, talentFrame.RectTransform, anchor: Anchor.Center, scaleBasis: ScaleBasis.BothHeight), style: null);
                GUIButton talentButton = new GUIButton(new RectTransform(Vector2.One, croppedTalentFrame.RectTransform, anchor: Anchor.Center), style: null)
                {
                    ToolTip = TalentMenu.GetTalentTooltip(talent, __instance.characterInfo),
                    UserData = talent.Identifier,
                    PressedColor = ___pressedColor,
                    Enabled = info.Character != null,
                    OnClicked = (button, userData) =>
                    {
                        if (isShowCaseTalent)
                        {
                            foreach (GUIComponent component in __instance.showCaseTalentFrames)
                            {
                                if (component.UserData is Identifier showcaseIdentifier && showcaseIdentifier == talentId)
                                {
                                    component.RectTransform.ScreenSpaceOffset = new Point((int)(button.Rect.Location.X - component.Rect.Width / 2f + button.Rect.Width / 2f), button.Rect.Location.Y - component.Rect.Height);
                                    component.Visible = true;
                                }
                                else
                                {
                                    component.Visible = false;
                                }
                            }

                            return true;
                        }

                        if (__instance.character is null) { return false; }

                        Identifier talentIdentifier = (Identifier)userData;
                        if (talentOption.MaxChosenTalents is 1)
                        {
                            // deselect other buttons in tier by removing their selected talents from pool
                            foreach (Identifier identifier in __instance.selectedTalents)
                            {
                                if (__instance.character.HasTalent(identifier) || identifier == talentId) { continue; }

                                if (talentOptionIdentifiers.Contains(identifier))
                                {
                                    __instance.selectedTalents.Remove(identifier);
                                }
                            }
                        }

                        if (__instance.character.HasTalent(talentIdentifier))
                        {
                            return true;
                        }
                        else if (IsViableTalentForCharacter(info.Character, talentIdentifier, __instance.selectedTalents))
                        {
                            if (!__instance.selectedTalents.Contains(talentIdentifier))
                            {
                                __instance.selectedTalents.Add(talentIdentifier);
                            }
                            else
                            {
                                __instance.selectedTalents.Remove(talentIdentifier);
                            }
                        }
                        else
                        {
                            __instance.selectedTalents.Remove(talentIdentifier);
                        }

                        __instance.UpdateTalentInfo();
                        return true;
                    },
                };

                talentButton.Color = talentButton.HoverColor = talentButton.PressedColor = talentButton.SelectedColor = talentButton.DisabledColor = Color.Transparent;

                GUIComponent iconImage;
                if (talent.Icon is null)
                {
                    iconImage = new GUITextBlock(new RectTransform(Vector2.One, talentButton.RectTransform, anchor: Anchor.Center), text: "???", font: GUIStyle.LargeFont, textAlignment: Alignment.Center, style: null)
                    {
                        OutlineColor = GUIStyle.Red,
                        TextColor = GUIStyle.Red,
                        PressedColor = ___unselectableColor,
                        DisabledColor = ___unselectableColor,
                        CanBeFocused = false,
                    };
                }
                else
                {
                    iconImage = new GUIImage(new RectTransform(Vector2.One, talentButton.RectTransform, anchor: Anchor.Center), sprite: talent.Icon, scaleToFit: true)
                    {
                        Color = talent.ColorOverride.TryUnwrap(out Color color) ? color : Color.White,
                        PressedColor = ___unselectableColor,
                        DisabledColor = ___unselectableColor * 0.5f,
                        CanBeFocused = false,
                    };
                }

                iconImage.Enabled = talentButton.Enabled;
                if (isShowCaseTalent)
                {
                    showCaseTalentButtonsToAdd.Add(talentId, iconImage);
                    continue;
                }

                buttonsToAdd.Add(new TalentButton(iconImage, talent));
            }

            foreach (TalentButton button in buttonsToAdd)
            {
                __instance.talentButtons.Add(button);
            }

            foreach (var (key, value) in showCaseTalentButtonsToAdd)
            {
                HashSet<TalentButton> buttons = new();
                foreach (Identifier identifier in talentOption.ShowCaseTalents[key])
                {
                    if (__instance.talentButtons.FirstOrNull(talentButton => talentButton.Identifier == identifier) is not { } button) { continue; }

                    buttons.Add(button);
                }

                __instance.talentShowCaseButtons.Add(new TalentShowCaseButton(buttons.ToImmutableHashSet(), value));
            }

            __instance.talentCornerIcons.Add(new TalentCornerIcon(subTree.Identifier, index, cornerIcon, talentBackground, talentBackgroundHighlight));
            return false;
        }
    }
}