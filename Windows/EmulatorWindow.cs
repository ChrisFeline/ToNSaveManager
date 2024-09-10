
using System.Diagnostics;
using System.Text;

using ToNSaveManager.Models.Index;
using ToNSaveManager.Utils;
using static ToNSaveManager.Models.Index.ToNIndex;
// using EntryBase = ToNSaveManager.Models.Index.ToNIndex.EntryBase;
// using Terror = ToNSaveManager.Models.Index.ToNIndex.Terror;
// using IEntry = ToNSaveManager.Models.Index.ToNIndex.IEntry;
// using Map = ToNSaveManager.Models.Index.ToNIndex.Map;

namespace ToNSaveManager.Windows
{
    public partial class EmulatorWindow : Form {
        #region Sub Classes
        internal class ToNOperation {
            public ToNRoundType RoundType = ToNRoundType.Classic;
            public ToNIndex.Terror? TerrorIndex;
            public ToNIndex.Terror? TerrorIndex2;
            public ToNIndex.Terror? TerrorIndex3;
            public ToNIndex.Map? MapIndex;
            public bool IsSaboteur = false; // Only on sabotage

            public bool IsHidden => RoundType == ToNRoundType.Eight_Pages ||
                RoundType == ToNRoundType.Fog || RoundType == ToNRoundType.Fog_Alternate;
        }

        internal class RoundTypeProxy {
            public string DisplayName;
            public ToNRoundType Value;

            public RoundTypeProxy(ToNRoundType roundType) {
                DisplayName = roundType == ToNRoundType.Intermission ? "Random" : roundType.ToString();
                Value = roundType;
            }

            public override string ToString() {
                return ((int)Value).ToString().PadRight(3) + " | " + DisplayName;
            }
        }
        #endregion

        internal static EmulatorWindow Instance = new EmulatorWindow();

        internal static ToNOperation Operation = new ToNOperation();
        internal static ToNIndex Database => ToNIndex.Instance;

        #region Index Properties
        internal static Map[] Maps;
        internal static Terror[] Terrors;
        internal static Terror[] Alternates;
        internal static Terror[] Whitelisted;

        internal static Terror[] Unbound;

        internal static Terror[] EightPages;
        internal static Map[] Maps8P;

        internal static Dictionary<ToNRoundType, int> RoundTypeToMapID = new Dictionary<ToNRoundType, int>() {
            { ToNRoundType.RUN,       254},
            { ToNRoundType.Cold_Night, 37},
            { ToNRoundType.GIGABYTE,    2},
            { ToNRoundType.Twilight,   67},
        };

        #endregion

        #region Initialization
        static EmulatorWindow() {
            Maps = Database.Maps.Values.Where((t, i) => i < Database.Maps.Count - 1).ToArray();
            Terrors = Database.Terrors.Values.ToArray();
            Alternates = Database.Alternates.Values.ToArray();
            Whitelisted = Terrors.Where(t => !t.CantBB).ToArray();

            Unbound = Database.Unbound.Values.ToArray();

            var redirect = Database.EightPRedirect;
            var eightP = Database.EightPages.ToDictionary();

            foreach (var pair in redirect) {
                int key = pair.Key;
                int group = pair.Value[0];
                int terror = pair.Value[1];

                Terror t = Database.GetTerror(terror, group);
                eightP[key] = new Terror() {
                    CantBB = t.CantBB, // Can't participate in bb
                    Group = t.Group,
                    // Inherited
                    Id = key,
                    Color = t.Color,
                    Name = t.Name
                };
            }

            EightPages = eightP.Values.ToArray();
            Maps8P = Maps.Where(m => m.EightP).ToArray();
        }

        public EmulatorWindow() {
            InitializeComponent();
        }
        #endregion

        #region UI Code
        internal static readonly ComboBox[] TerrorComboBoxes = new ComboBox[3];

        public static void Open(Form parent) {
            if (Instance == null || Instance.IsDisposed) Instance = new();

            if (Instance.Visible) {
                Instance.BringToFront();
                return;
            }

            Instance.StartPosition = FormStartPosition.Manual;
            Instance.Location = new Point(
                parent.Location.X + (parent.Width - Instance.Width) / 2,
                Math.Max(parent.Location.Y + (parent.Height - Instance.Height) / 2, 0)
            );
            Instance.Show(parent);
        }

        private void Form1_Load(object sender, EventArgs e) {
            if (Database.Maps.Count < 1) {
                MessageBox.Show("Invalid Database", "Invalid Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            SetMonsterDataSource(null);
            SetLocationDataSource(null);

            TerrorComboBoxes[0] = comboMonster;
            TerrorComboBoxes[1] = comboMonster2;
            TerrorComboBoxes[2] = comboMonster3;

            var values = ((ToNRoundType[])Enum.GetValues(typeof(ToNRoundType))).Select(rt => new RoundTypeProxy(rt)).ToArray();
            comboRoundType.DataSource = values;
            comboRoundType.SelectedIndex = 0;

            buttonStep_Reset();
            UpdateSpecialCheckbox();
        }

        private void UpdateSpecialCheckbox() {
            checkSpecial.Visible = HasValidEncounter(Operation.TerrorIndex) || HasValidEncounter(Operation.TerrorIndex2) || HasValidEncounter(Operation.TerrorIndex3);
            if (checkSpecial.Visible) {
                checkSpecial.Text = LastEncounter.Name;
            } else checkSpecial.Checked = false;
        }

        private Terror.Encounter LastEncounter;
        private bool HasValidEncounter(Terror? terror) {
            if (terror != null && terror.Encounters != null && terror.Encounters.Length > 0 && (terror.Encounters[0].RoundType == ToNRoundType.Intermission || terror.Encounters[0].RoundType == Operation.RoundType)) {
                LastEncounter = terror.Encounters[0];
                return true;
            }

            return false;
        }

        private void SetMonsterDataSource(Terror[]? source) {
            SetComboDataSource(comboMonster, source);
        }

        private void SetLocationDataSource(Map[]? source) {
            SetComboDataSource(comboLocation, source);
        }

        private void SetComboDataSource<T>(ComboBox comboBox, T[]? source) where T : IEntry {
            comboBox.DataSource = null;
            comboBox.Enabled = source != null;
            if (source == null) return;

            T[] __source = new T[source.Length + 1];
            __source[0] = Activator.CreateInstance<T>();

            T v = __source[0];
            v.Id = -1;
            v.Name = "_RANDOM_";
            v.Color = Color.White;
            __source[0] = v;

            Array.Copy(source, 0, __source, 1, source.Length);

            comboBox.DataSource = __source;
            comboBox.SelectedIndex = 0;
        }

        private void comboRoundType_SelectedIndexChanged(object sender, EventArgs e) {
            RoundTypeProxy? selectedValue = (RoundTypeProxy?)comboRoundType.SelectedValue;

            ToNRoundType roundType;
            if (selectedValue == null) roundType = ToNRoundType.Classic;
            else roundType = selectedValue.Value;


            if (Operation.RoundType != roundType) {
                Operation.RoundType = roundType;
                int roundTypeInt = (int)roundType;

                switch (roundType) {
                    case ToNRoundType.Eight_Pages:
                        SetMonsterDataSource(EightPages);
                        SetLocationDataSource(Maps8P);
                        break;

                    case ToNRoundType.Unbound:
                        SetMonsterDataSource(Unbound);
                        SetLocationDataSource(Maps);
                        break;

                    // These have map pools
                    case ToNRoundType.Intermission: // Dont do anything
                    case ToNRoundType.RUN:
                    case ToNRoundType.Cold_Night:
                    case ToNRoundType.GIGABYTE:
                    case ToNRoundType.Twilight:
                        SetMonsterDataSource(null);
                        SetLocationDataSource(null);
                        break;
                    case ToNRoundType.Solstice:
                    case ToNRoundType.Mystic_Moon:
                    case ToNRoundType.Blood_Moon:
                    case ToNRoundType.Custom:
                        SetMonsterDataSource(null);
                        SetLocationDataSource(Maps);
                        break;
                    case ToNRoundType.Classic:
                    case ToNRoundType.Fog:
                    case ToNRoundType.Fog_Alternate:
                    case ToNRoundType.Ghost_Alternate:
                    case ToNRoundType.Ghost:
                    case ToNRoundType.Punished:
                    case ToNRoundType.Alternate:
                    case ToNRoundType.Sabotage:
                    case ToNRoundType.Cracked:
                        SetMonsterDataSource(
                            roundType == ToNRoundType.Alternate ||
                            roundType == ToNRoundType.Fog_Alternate ||
                            roundType == ToNRoundType.Ghost_Alternate
                            ? Alternates : Terrors
                        );
                        SetLocationDataSource(Maps);
                        break;
                    case ToNRoundType.Bloodbath:
                    case ToNRoundType.Double_Trouble:
                    case ToNRoundType.EX:
                    case ToNRoundType.Midnight:
                        SetMonsterDataSource(Whitelisted);
                        SetLocationDataSource(Maps);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                SetComboDataSource(comboMonster2, roundTypeInt > 5 && roundTypeInt < 8 || roundType == ToNRoundType.Midnight ? Whitelisted : null);
                SetComboDataSource(comboMonster3, roundTypeInt > 5 && roundTypeInt < 7 ? Terrors :
                    (roundType == ToNRoundType.Midnight ? Alternates : null));

                checkBoxIsKiller.Visible = roundType == ToNRoundType.Sabotage;
            }
        }

        private void SetFromCombo<T>(ComboBox element, ref T? indexOutput) where T : IEntry {
            int index = element.SelectedIndex;
            object? selectedItem = element.SelectedItem;
            if (index > -1 && selectedItem != null) {
                indexOutput = (T)selectedItem;
            } else indexOutput = default;
            Logger.Debug($"Selected: {indexOutput} | {element.Name}");

            UpdateSpecialCheckbox();
        }

        private void comboMonster_SelectedIndexChanged(object sender, EventArgs e) {
            SetFromCombo<Terror>((ComboBox)sender, ref Operation.TerrorIndex);
        }

        private void comboMonster2_SelectedIndexChanged(object sender, EventArgs e) {
            SetFromCombo<Terror>((ComboBox)sender, ref Operation.TerrorIndex2);
        }

        private void comboMonster3_SelectedIndexChanged(object sender, EventArgs e) {
            SetFromCombo<Terror>((ComboBox)sender, ref Operation.TerrorIndex3);
        }

        private void comboLocation_SelectedIndexChanged(object sender, EventArgs e) {
            SetFromCombo<Map>((ComboBox)sender, ref Operation.MapIndex);
        }

        private void comboMonster_DrawItem(object sender, DrawItemEventArgs e) {
            if (e.Index < 0)
                return;

            // Get the ComboBox
            ComboBox comboBox = (ComboBox)sender;

            EntryBase item = (EntryBase?)comboBox.Items[e.Index] ?? Terror.Empty;

            // Get the item text and color
            string? itemText = item.Id.ToString().PadRight(3) + " | " + item.ToString();
            Color textColor = item.Color;

            // Draw the item background
            // e.DrawBackground();
            using (SolidBrush brush = new SolidBrush(comboBox.BackColor))
                e.Graphics.FillRectangle(brush, e.Bounds);

            // Draw the item text
            TextRenderer.DrawText(e.Graphics, itemText, e.Font, e.Bounds, textColor,
                TextFormatFlags.VerticalCenter);

            // Draw the focus rectangle if the item has focus
            e.DrawFocusRectangle();
        }

        private void comboMonster_EnabledChanged(object sender, EventArgs e) {
            ComboBox comboBox = (ComboBox)sender;
            comboBox.DrawMode = comboBox.Enabled ? DrawMode.OwnerDrawFixed : DrawMode.Normal;
            // if (comboBox != comboMonster) comboBox.Visible = comboBox.Enabled;
        }
        #endregion

        private void buttonStep_Reset() {
            mainPanel.Enabled = true;
            buttonStepStart.Enabled = true;
            buttonStepKillerSet.Enabled = false;
            buttonStepReveal.Enabled = false;
            buttonStepEndRound.Enabled = false;
        }

        private void buttonStep_Click(object sender, EventArgs e) {
            Button button = (Button)sender;
            switch (button.TabIndex) {
                case 0: // Start
                    mainPanel.Enabled = false;
                    buttonStepStart.Enabled = false;
                    buttonStepKillerSet.Enabled = true;
                    buttonStepEndRound.Enabled = true;
                    OnRoundStart();
                    break;

                case 1: // Set
                    buttonStepKillerSet.Enabled = false;
                    buttonStepReveal.Enabled = Operation.IsHidden;
                    OnRoundSetKillers(!buttonStepReveal.Enabled);
                    break;

                case 2: // Reveal
                    buttonStepReveal.Enabled = false;
                    OnRoundSetKillers(true);
                    break;

                default:
                    buttonStep_Reset();
                    OnRoundEnd();
                    break;
            }
        }

        private ToNRoundType CurrentRoundType;
        private bool CurrentIsKiller => CurrentRoundType == ToNRoundType.Sabotage && checkBoxIsKiller.Checked;

        private void OnRoundStart() {
            // Round is random
            if (comboRoundType.SelectedIndex == 0)
                comboRoundType.SelectedIndex = Random.Shared.Next(1, comboRoundType.Items.Count);

            RoundTypeProxy roundTypeProxy = (RoundTypeProxy?)comboRoundType.SelectedItem ?? new RoundTypeProxy(ToNRoundType.Classic);
            CurrentRoundType = roundTypeProxy.Value;

            Map selectedMap = Map.Empty;
            if (comboLocation.Items.Count > 0) {
                if (comboLocation.SelectedIndex == 0) comboLocation.SelectedIndex = Random.Shared.Next(1, comboLocation.Items.Count);
                selectedMap = (Map?)comboLocation.SelectedItem ?? selectedMap;
            } else if (RoundTypeToMapID.TryGetValue(roundTypeProxy.Value, out int mapId)) selectedMap = Database.GetMap(mapId);

            if (selectedMap.IsEmpty) throw new Exception("Failed to emulate map.");

            LilOSC.SetOptInStatus(true);
            LilOSC.SetMap(selectedMap);
            LilOSC.SetTerrorMatrix(new TerrorMatrix(CurrentRoundType == ToNRoundType.GIGABYTE ? ToNRoundType.Classic : CurrentRoundType) { IsSaboteur = CurrentIsKiller });
        }

        private void OnRoundSetKillers(bool reveal = true) {
            if (CurrentRoundType == ToNRoundType.Custom) return;

            int[] terrors = new int[TerrorComboBoxes.Length];
            for (int i = 0; i < TerrorComboBoxes.Length; i++) {
                if (reveal) {
                    var comboBox = TerrorComboBoxes[i];
                    if (comboBox.Items.Count > 0) {
                        if (comboBox.SelectedIndex == 0) comboBox.SelectedIndex = Random.Shared.Next(1, comboBox.Items.Count);
                        terrors[i] = ((Terror?)comboBox.SelectedItem ?? Terror.Zero).Id;
                    } else {
                        terrors[i] = CurrentRoundType == ToNRoundType.Double_Trouble || CurrentRoundType == ToNRoundType.EX ? terrors[i-1] : 0;
                    }
                } else terrors[i] = byte.MaxValue;
            }

            if (CurrentRoundType == ToNRoundType.Bloodbath) {
                if (terrors.All(v => v == terrors[0])) CurrentRoundType = ToNRoundType.EX;
                else if (terrors.Length != terrors.Distinct().Count()) CurrentRoundType = ToNRoundType.Double_Trouble;
            }

            TerrorMatrix terrorMatrix = new TerrorMatrix(CurrentRoundType.ToString().Replace('_', ' ').Replace(" Alternate", " (Alternate)"), terrors);
            terrorMatrix.IsSaboteur = CurrentIsKiller;
            terrorMatrix.RoundType = CurrentRoundType;

            if (CurrentRoundType == ToNRoundType.GIGABYTE) {
                terrorMatrix.RoundType = CurrentRoundType;
                terrorMatrix.Terrors = [ new TerrorInfo(1, TerrorGroup.Events) ];
                terrorMatrix.TerrorCount = 1;
            }

            // TODO: Check for specials
            if (checkSpecial.Visible && checkSpecial.Checked) terrorMatrix.MarkEncounter();

            LilOSC.SetTerrorMatrix(terrorMatrix);
        }

        private void OnRoundEnd() {
            LilOSC.SetOptInStatus(false);
            LilOSC.SetMap(Map.Empty);
            LilOSC.SetTerrorMatrix(TerrorMatrix.Empty);
        }
    }
}