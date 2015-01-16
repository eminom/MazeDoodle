using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DoodleControls;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace MazeDoodle
{
    public partial class EditorForm : Form
    {
        public EditorForm()
        {
            InitializeComponent();
            dcPanel_.EvtDoodleMm_ += new DoodleMouseMove(OnDoodleMouseMove);
            dcPanel_.EvtDoodleMc_ += new DoodleMouseClick(OnDoodleMouseClick);
            dcPanel_.EvtTrapSetChanged_ += new DoodleTrapSetChanged(OnTrapSetChanged);
            dcPanel_.EvtCameraSetChanged_ += new DoodleCameraSetChanged(UpdateCameraList);
            dcPanel_.EvtControlStateChanged_ += new DoodleControlStateChanged(OnControlStateChanged);
            dcPanel_.EvtRemoteControlChanged_ += new DoodleRemoteControlChanged(OnRemoteControlChanged);
            dcPanel_.EvtLogicWallViewPropertyChanged += new DoodleLogicWallViewPropertyChanged(OnLogicWallViewChanged);

            int r,c;

            dcPanel_.GetStartPoint(out r,out c);
            startPointColumnTextBox_.Text = c.ToString();
            startPointRowTextBox_.Text = r.ToString();
        }

        private void OnTrapSetChanged()
        {
            UpdateTrapList();
        }

        private void OnDoodleMouseMove(int x, int y)
        {
            tssCurrentPos_.Text = String.Format("(R:{0}, C:{1})", y, x);
        }

        private void OnDoodleMouseClick(int x, int y)
        {
            if (wallCheckBtn_.Checked)
            {
                dcPanel_.ToggleWallAt(x, y);
            }
            else if (key1CheckBtn.Checked)
            {
                dcPanel_.PutItemAt(x, y, 1, GetComboSpecialtyString());
                key1CheckBtn.Checked = false;
                statusStrip_.Text = "Key1 set";
            }
            else if (key2CheckBtn.Checked)
            {
                dcPanel_.PutItemAt(x, y, 2, GetComboSpecialtyString());
                key2CheckBtn.Checked = false;
                statusStrip_.Text = "Key2 set";
            }
            else if (key3CheckBtn.Checked)
            {
                dcPanel_.PutItemAt(x, y, 3, GetComboSpecialtyString());
                key3CheckBtn.Checked = false;
                statusStrip_.Text = "Key3 set";
            }
            else if(keyXCheck.Checked)
            {
                try
                {
                    int key = int.Parse(keyTextBox_.Text);
                    dcPanel_.PutItemAt(x, y, key, GetComboSpecialtyString());
                }
                catch(FormatException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.Beep();
                    //FlashWindow(this.Handle, true);
                }
            }
            else if (pickStartPointChkBox_.Checked)
            {
                startPointRowTextBox_.Text = y.ToString();
                startPointColumnTextBox_.Text = x.ToString();
                dcPanel_.SetStartPoint(y, x);

                pickStartPointChkBox_.Checked = false;  //~ reset
            }
            else if (removeItemCheckBtn.Checked)
            {
                dcPanel_.ClearAtPos(x, y);
                statusStrip_.Text = "Clear!";
            }
            else if (stairCheckBtn_.Checked)
            {
                try
                {
                    int stair_port = int.Parse(stairportTextBox_.Text);
                    int item_req = int.Parse(itemReqBox_.Text);
                    dcPanel_.SetStairPortAt(x, y, stair_port, item_req, true);
                }
                catch (FormatException)
                {
                    //System.Media.SystemSounds.Beep.Play();
                    //SystemSounds.Beep.Play();
                    Console.Beep();
                    //FlashWindow(this.Handle,true);
                }
            }
            else if (rockCheckBtn_.Checked)
            {
                dcPanel_.SetRockAt(x, y);
            }
            else if (removeRockCheckBtn_.Checked)
            {
                dcPanel_.RemoveRockAt(x, y);
            }
            else if (textureCheckBox_.Checked)
            {
                //int t;
                //if (int.TryParse(textureTextBox_.Text, out t))
                //{
                //    //if (t >= 1 && t <= 2)
                //    //{
                //    //    dcPanel_.SetTexture(x, y, t);
                //    //}
                //    dcPanel_.SetTexture(x, y, t);
                //}

                dcPanel_.SetTexture(x, y, TerranceRender.EncodeTextureId(texChooser.TexRow,texChooser.TexColum));
            }
            else if (removeTextureCheckBox_.Checked)
            {
                dcPanel_.RemoveTexture(x, y);
            }
            else if (pickShadowChkBtn.Checked)
            {
                dcPanel_.SetShadowPointAt(x, y);
                UpdateShadowPointList();
            }
        }

        private void UpdateUiText()
        {
            int r, c;
            dcPanel_.GetSize(out r, out c);
            tssVolumn_.Text = String.Format("Size::{0} x {1}", r, c);

            dcPanel_.GetStartPoint(out r, out c);
            startPointColumnTextBox_.Text = c.ToString();
            startPointRowTextBox_.Text = r.ToString();

            UpdateShadowPointList();
            UpdateKeeperInfoList();
            UpdateTrapList();
            UpdateCameraList();
            UpdateRemoteControlsList();
            //startPointItemRequiredTextBox_.Text = 
        }

        private String GetComboSpecialtyString()
        {
            String rv = "";
            if (cbItemSpecialty.SelectedIndex >= 0)
            {
                rv = cbItemSpecialty.Items[cbItemSpecialty.SelectedIndex].ToString();
            }
            return rv;
        }

        private void editorForm_Load(object sender, EventArgs e)
        {
            UpdateUiText();
        }

        //private void cbRow_TextChanged(object sender, EventArgs e)
        //{
        //    TryUpdateRowColumn();
        //}

        //private void cbColumn_TextChanged(object sender, EventArgs e)
        //{
        //    TryUpdateRowColumn();

        //}

        //private void TryUpdateRowColumn()
        //{
        //    try
        //    {
        //        int newRow = 6;// Int32.Parse(cbRow_.Text);
        //        int newColumn = 6;// Int32.Parse(cbColumn_.Text);

        //        int or, oc;
        //        dcPanel_.GetSize(out or, out oc);
        //        if (newRow != or || newColumn != oc)
        //        {
        //            dcPanel_.ResetSize(newRow, newColumn);
        //        }
        //    }
        //    catch (FormatException)
        //    {
        //        Console.WriteLine("Parameter is invalid");
        //    }
        //}

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String file_dir = RecentVisited();
            if( file_dir.Length != 0 )
                file_dir = System.IO.Path.GetDirectoryName(file_dir);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = file_dir;
            if (DialogResult.OK == sfd.ShowDialog())
            {
                saveToFileWithName(sfd.FileName);
            }
        }

        private void saveToFileWithName(String file_name)
        {
            Console.WriteLine(file_name);
            dcPanel_.WriteToJson(file_name);
            RecordRecentVisited(file_name);
            SetEditorTitle(file_name, true);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewForm newForm = new NewForm();
            if (DialogResult.OK == newForm.ShowDialog(this))
            {
                //~ 
                dcPanel_.ResetData(newForm.RowCount,newForm.ColumnCount);
                SetEditorTitle("New Map",false);
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String file = RecentVisited();
            if (file.Length != 0)
                file = System.IO.Path.GetDirectoryName(file);

            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.FileName = file;//defaultFilePath;
            ofd.InitialDirectory = file;
            if( DialogResult.OK == ofd.ShowDialog() )
            {
                dcPanel_.LoadFromJson(ofd.FileName);
                RecordRecentVisited(ofd.FileName);
                stripStatusLabel_.Text = "Load compelte";
                UpdateUiText();
                SetEditorTitle(ofd.FileName,true);
            }
        }

        private void SetEditorTitle(String title_name,bool is_file_name)
        {
            this.Text = title_name;
            if (is_file_name)
                current_file_name_ = title_name;
            else
                current_file_name_ = "";
        }

        private const String defaultFilePath = @"E:\GameDevelop\Eclipse\MagicTower\res\level\maze0.json";


        private void SetCheckOnly(CheckBox cb)
        {
            CheckBox[] cbs = new CheckBox[]{this.key1CheckBtn,this.key2CheckBtn,this.key3CheckBtn,this.keyXCheck,
               this.wallCheckBtn_, this.removeItemCheckBtn,this.stairCheckBtn_,
                this.rockCheckBtn_,
                this.removeRockCheckBtn_,
                this.pickStartPointChkBox_,
                this.textureCheckBox_,
                this.removeTextureCheckBox_,
                this.pickShadowChkBtn
            };
            for (int i = 0; i < cbs.Length; ++i)
            {
                cbs[i].Checked = (cb == cbs[i]);
                //EventHandler eh = new EventHandler(cbs[i].CheckedChanged);
                //cbs[i].CheckedChanged = new EventHandler();
            }
        }

        private void key1CheckBtn_CheckedChanged(object sender, EventArgs e)
        {
            if(sender == key1CheckBtn && key1CheckBtn.Checked)
                SetCheckOnly(key1CheckBtn);
        }

        private void key2CheckBtn_CheckedChanged(object sender, EventArgs e)
        {
            if(sender == key2CheckBtn && key2CheckBtn.Checked)
                SetCheckOnly(key2CheckBtn);
        }

        private void key3CheckBtn_CheckedChanged(object sender, EventArgs e)
        {
            if( sender == key3CheckBtn && key3CheckBtn.Checked)
                SetCheckOnly(key3CheckBtn);
        }

        private void wallCheckBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == wallCheckBtn_ && wallCheckBtn_.Checked)
            {
                SetCheckOnly(wallCheckBtn_);
                dcPanel_.SetEnableLogicWall(true);
            }
        }

        private void removeItemCheckBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == removeItemCheckBtn && removeItemCheckBtn.Checked)
                SetCheckOnly(removeItemCheckBtn);
        }

        private void stairCheckBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == stairCheckBtn_ && stairCheckBtn_.Checked)
                SetCheckOnly(stairCheckBtn_);
        }

        private void keyXCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == keyXCheck && keyXCheck.Checked)
                SetCheckOnly(keyXCheck);

        }

        private void encloseBtn_Click(object sender, EventArgs e)
        {
            dcPanel_.Enclose();
        }


        private void discloseBtn_Click(object sender, EventArgs e)
        {
            dcPanel_.Disclose();
        }

        private void textureCheckBox__CheckedChanged(object sender, EventArgs e)
        {
            if( sender == textureCheckBox_ && textureCheckBox_.Checked)
                SetCheckOnly(textureCheckBox_);
        }

        private void removeTextureCheckBox__CheckedChanged(object sender, EventArgs e)
        {
            if (sender == removeTextureCheckBox_ && removeTextureCheckBox_.Checked)
                SetCheckOnly(removeTextureCheckBox_);
        }

        private void rockCheckBtn__CheckedChanged(object sender, EventArgs e)
        {
            if (sender == rockCheckBtn_ && rockCheckBtn_.Checked)
            {
                SetCheckOnly(rockCheckBtn_);
                dcPanel_.SetEnableLogicWall(true);
            }
        }

        private void removeRockCheckBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == removeRockCheckBtn_ && removeRockCheckBtn_.Checked)
            {
                SetCheckOnly(removeRockCheckBtn_);
                dcPanel_.SetEnableLogicWall(true);
            }
        }

        private void pickShadowChkBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == pickShadowChkBtn && pickShadowChkBtn.Checked)
                SetCheckOnly(pickShadowChkBtn);
        }

        private void pickPointChkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == pickStartPointChkBox_ && pickStartPointChkBox_.Checked)
                SetCheckOnly(pickStartPointChkBox_);
        }

        private String GetInfoFile()
        {
            String cur = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            cur = System.IO.Path.GetDirectoryName(cur);
            return cur+@"\MazeInfo.ini";
        }

        private String RecentVisited()
        {
            string ini = GetInfoFile();
            int sz = 1024;
            StringBuilder sb = new StringBuilder(sz);
            int ret = GetPrivateProfileString(IniSection,KeyRecentVisited,"",sb,sz,ini);
            return sb.ToString();
        }

        private void RecordRecentVisited(String full_path)
        {
            string ini = GetInfoFile();
            WritePrivateProfileString(IniSection, KeyRecentVisited, full_path, ini);
        }

        private void keepersList_Click(object sender, EventArgs e)
        {
            int selected = keepersList_.SelectedIndex;
            dcPanel_.SetActiveKeeper(selected);

            UpdateKeeperDetails();
        }

        private void UpdateShadowPointList()
        {
            shadowPointList_.Items.Clear();
            List<ShadowPoint> shadows = dcPanel_.GetShadowPointList();
            foreach (ShadowPoint sp in shadows)
            {
                shadowPointList_.Items.Add(sp.ToString());
            }
        }

        private void routeTypeGroupCheckChanged(object sender, EventArgs e)
        {
            int selected = keepersList_.SelectedIndex;
            List<DoodleKeeper> dis = dcPanel_.GetKeeperList();
            if (selected >= 0 && selected < dis.Count)
            {
                DoodleKeeper dk = dis[selected];
                if (radBtnRouteDefault.Checked)
                    dk.SetRouteType("default");
                else if(radBtnRouteForard.Checked)
                    dk.SetRouteType("forward");
            }
            else
            {
                radBtnRouteDefault.Checked = false;
                radBtnRouteForard.Checked = false;
            }
        }

        private void UpdateKeeperInfoList()
        {
            //~ keepers' list
            keepersList_.Items.Clear();
            List<DoodleKeeper> keepers = dcPanel_.GetKeeperList();
            foreach (DoodleKeeper dk in keepers)
            {
                int pos = keepersList_.Items.Add(dk.GetShortName());
            }

            UpdateKeeperDetails();
        }

        private void UpdateTrapList()
        {
            listViewTrap.Items.Clear();
            for(int i=0;i<dcPanel_.TrapSetCount;++i){
                List<TrapSetPoint> lts = dcPanel_.GetTrapGroupByIndex(i);
                foreach(TrapSetPoint tsp in lts){
                    ListViewItem item = listViewTrap.Items.Add(tsp.ToString());
                    item.SubItems.Add(i.ToString());
                }
            }
        }

        private void UpdateCameraList()
        {
            listViewCameras.Items.Clear();
            for (int i = 0; i < dcPanel_.SecurityCameraCount; ++i)
            {
                SecurityCameraEquip sce = dcPanel_.GetCameraByIndex(i);
                ListViewItem item = listViewCameras.Items.Add(sce.ToString());
                item.SubItems.Add(sce.Period);
            }
        }

        private void UpdateRemoteControlsList()
        {
            lvRemoteControls.Items.Clear();
            IEnumerator<DoodleControls.RemoteControl> enumerator = dcPanel_.RemoteControlEnumerator;
            while (enumerator.MoveNext())
            {
                DoodleControls.RemoteControl rc = enumerator.Current;
                ListViewItem lvi = lvRemoteControls.Items.Add(rc.FormatGateName);
                lvi.SubItems.Add(rc.FormatEqSetString);
            }

        }

        private void OnControlStateChanged(DoodleControlState state)
        {
            //~ reset all
            btnAddOneCamera.Text = "Add Camera";
            addTrapBtn.Text = "Add Trap";
            btnAddRemoteControl.Text = "Add R-Ctrl";

            switch (state)
            {
                case DoodleControlState.PickingCamera:
                    btnAddOneCamera.Text = "Adding Camera...";
                    break;
                case DoodleControlState.PickingTrap:
                    addTrapBtn.Text = "Adding Trap...";
                    break;
                case DoodleControlState.PickingRemoteControl:
                    btnAddRemoteControl.Text = "Adding R-Ctrl...";
                    break;
                case DoodleControlState.Idle:
                default:
                    break;
            }
        }

        private void OnRemoteControlChanged()
        {
            UpdateRemoteControlsList();
        }

        private void OnLogicWallViewChanged(bool logic_wall_view_enabled)
        {
            //~
            logicGridToolStripMenuItem.Checked = logic_wall_view_enabled;

            if (!logic_wall_view_enabled)
            {
                wallCheckBtn_.Checked = false;
                rockCheckBtn_.Checked = false;
                removeRockCheckBtn_.Checked = false;
            }
        }

        private void btnAddOneCamera_Click(object sender, EventArgs e)
        {
            /*dcPanel_.SetControlState(DoodleControlState.PickingCamera);*/
            dcPanel_.FlipPickingCamera();
        }

        private void btnRemoveAllCameras_Click(object sender, EventArgs e)
        {
            dcPanel_.RemoveAllCameras();
            UpdateCameraList();
        }

        private void btnRemoveOneCamera_Click(object sender, EventArgs e)
        {
            if (listViewCameras.SelectedIndices.Count > 0)
            {
                int i = listViewCameras.SelectedIndices[0];
                SecurityCameraEquip sce = SecurityCameraEquip.Parse(listViewCameras.Items[i].Text);
                dcPanel_.RemoveCamera(sce);

                UpdateCameraList();
            }
        }

        private void addTrapBtn_Click(object sender, EventArgs e)
        {
            dcPanel_.FlipPickingTrap();
        }

        private void switchTrapGroupBtn_Click(object sender, EventArgs e)
        {
            if( listViewTrap.SelectedIndices.Count > 0 )
            {
                int i= listViewTrap.SelectedIndices[0];
                String name = listViewTrap.Items[i].Text;
                dcPanel_.SwitchTrapGroup(name);
                UpdateTrapList();       //~ again. the group should be changed.
            }
        }

        private void removeTrapBtn_Click(object sender, EventArgs e)
        {
            if( listViewTrap.SelectedIndices.Count > 0 )
            {
                int i = listViewTrap.SelectedIndices[0];
                String name = listViewTrap.Items[i].Text;
                dcPanel_.RemoveTrapByName(name);
                UpdateTrapList();       //~ again
            }
        }

        private void removeAllTrapsBtn_Click(object sender, EventArgs e)
        {
            dcPanel_.RevemoAllTrap();
            UpdateTrapList();           //~ again

        }


        private void UpdateKeeperDetails()
        {
            // keeper details
            keeperInfoList_.Items.Clear();
            int selected = keepersList_.SelectedIndex;
            List<DoodleKeeper> dis = dcPanel_.GetKeeperList();
            if (selected >= 0 && selected < dis.Count)
            {
                DoodleKeeper dk = dis[selected];
                foreach (String s in dk.Routes)
                {
                    keeperInfoList_.Items.Add(s);
                }

                switch (dk.RouteType)
                {
                    case "default":
                        radBtnRouteDefault.Checked = true;
                        break;
                    case "forward":
                        radBtnRouteForard.Checked = true;
                        break;
                }

                switch (dk.Velocity)
                {
                    default:
                    case 0:
                        cbSpeed.SelectedIndex = 0;
                        break;
                    case 1:
                        cbSpeed.SelectedIndex = 1;
                        break;
                }

                switch (dk.Skin)
                {
                    default:
                    case 0:
                        cbSkin.SelectedIndex = 0;
                        break;
                    case 1:
                        cbSkin.SelectedIndex = 1;
                        break;
                    case 2:
                        cbSkin.SelectedIndex = 2;
                        break;
                }
            }
            else
            {
                //~ clean the radio group of "routing type";
                radBtnRouteDefault.Checked = false;
                radBtnRouteForard.Checked = false;

            }
        }

        private void cbSkin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSkin.SelectedIndex >= 0)
            {
                String skinStr = cbSkin.Items[cbSkin.SelectedIndex].ToString();
                try
                {
                    int selected = keepersList_.SelectedIndex;
                    DoodleKeeper dk = dcPanel_.GetKeeperByIndex(selected);
                    dk.SetSkin(UiMapping.MapSkinStringToInt(skinStr));
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                catch (IndexOutOfRangeException)
                {

                }
            }
        }

        private void cbSpeed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbSpeed.SelectedIndex >= 0)
            {
                String veloStr = cbSpeed.Items[cbSpeed.SelectedIndex].ToString();
                try
                {
                    int selected = keepersList_.SelectedIndex;
                    DoodleKeeper dk = dcPanel_.GetKeeperByIndex(selected);
                    dk.SetVelocity(UiMapping.MapVeloStringToInt(veloStr));
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                catch (IndexOutOfRangeException)
                {
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Remove current keeper");
            int to_be_deleted = keepersList_.SelectedIndex;
            List<DoodleKeeper> dis  = dcPanel_.GetKeeperList();
            if (to_be_deleted >= 0 && to_be_deleted < dis.Count)
            {
                //~ one target aimed
                dis.RemoveAt(to_be_deleted);
                dcPanel_.SetActiveKeeper(-1);
                UpdateKeeperInfoList();
                dcPanel_.Invalidate();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //~ clear others
            SetCheckOnly(null);

            switch (dcPanel_.PickState)
            {
                case KeeperPickState.Idle:
                    btnAdd.Text = "End Editing";
                    dcPanel_.SetPickState(KeeperPickState.PickingStartPoint);
                    break;
                case KeeperPickState.PickingStartPoint:
                case KeeperPickState.PickingRoutePoint:
                    btnAdd.Text = "Add";
                    dcPanel_.SetPickState(KeeperPickState.Idle);
                    UpdateKeeperInfoList();
                    dcPanel_.Invalidate();
                    break;
            }
        }

        private void texChooser_Click(object sender, EventArgs e)
        {
            SelectTextureForm stf = new SelectTextureForm();
            if (DialogResult.OK == stf.ShowDialog(this))
            {
                texChooser.SetTexture(stf.ClickRow, stf.ClickCol);
                textureCheckBox_.Checked = true;
            }
        }

        private void resetShadowPointsBtn_Click(object sender, EventArgs e)
        {
            dcPanel_.ClearShadowPoints();
            UpdateShadowPointList();
        }

        private void cbItemSpecialty_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void btnAddRemoteControl_Click(object sender, EventArgs e)
        {
            lvRemoteControls.SelectedItems.Clear();
            dcPanel_.FlipAddRemoteControl();
            //dcPanel_.SetControlState(DoodleControlState.PickingRemoteControl);
        }

        private void btnRemoveOneRemoteControl_Click(object sender, EventArgs e)
        {
            if (lvRemoteControls.SelectedIndices.Count > 0)
            {
                int sel = lvRemoteControls.SelectedIndices[0];
                dcPanel_.RemoveRemoteControlByIndex(sel);
            }
        }

        private void btnClearAllRemoteControl_Click(object sender, EventArgs e)
        {
            dcPanel_.RemoveAllRemoteControls();

        }

        private void lvRemoteControls_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvRemoteControls.SelectedIndices.Count > 0)
            {
                dcPanel_.SelectRemoteControlByIndex(lvRemoteControls.SelectedIndices[0]);
            }

        }

        private const String IniSection = "Pro";
        private const String KeyRecentVisited = "RecentVisited";
        private String current_file_name_ = "";
        private readonly Regex period_pattern_ = new Regex(@"^[0-3]+$", RegexOptions.Compiled | RegexOptions.Singleline);

        [DllImport("user32")]
        private static extern bool FlashWindow(IntPtr hWnd, bool invert);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
                                                             string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key,
                                                          string def, StringBuilder retVal,
                                                          int size, string filePath);

        private void radBtnAllLayer_CheckedChanged(object sender, EventArgs e)
        {
            if (radBtnLayerBackground.Checked)
            {
                dcPanel_.SetTexEditMask(DoodleCake.LayerBackground);
                chkBtnViewBackground.Checked = true;            //~ and the change handler will follow
            }
            else if (radBtnLayerDecoration.Checked)
            {
                dcPanel_.SetTexEditMask(DoodleCake.LayerDecoration);
                chkBtnViewDecoration.Checked = true;
            }
            else if (radBtnLayerRoleAndItem.Checked)
            {
                dcPanel_.SetTexEditMask(DoodleCake.LayerRoleAndItem);
                chkBtnViewRoleAndItem.Checked = true;
            }
            else if (radBtnLayerFix.Checked)
            {
                dcPanel_.SetTexEditMask(DoodleCake.LayerFix);
                chkBtnViewFixLayer.Checked = true;
            }
        }

        private void chkBtnViewBackground_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBtnViewBackground.Checked)
                dcPanel_.EnableTexViewMask(DoodleCake.LayerBackground);
            else
                dcPanel_.DisableTexViewMask(DoodleCake.LayerBackground);

            if (chkBtnViewDecoration.Checked)
                dcPanel_.EnableTexViewMask(DoodleCake.LayerDecoration);
            else
                dcPanel_.DisableTexViewMask(DoodleCake.LayerDecoration);

            if( chkBtnViewRoleAndItem.Checked)
                dcPanel_.EnableTexViewMask(DoodleCake.LayerRoleAndItem);
            else
                dcPanel_.DisableTexViewMask(DoodleCake.LayerRoleAndItem);

            if( chkBtnViewFixLayer.Checked)
                dcPanel_.EnableTexViewMask(DoodleCake.LayerFix);
            else
                dcPanel_.DisableTexViewMask(DoodleCake.LayerFix);
        }

        private void saveSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == current_file_name_ || 0 == current_file_name_.Length)
            {
                Console.WriteLine("No current file saved.");
                stripStatusLabel_.Text = "Cannot save right now.Try Save-As";
                return;
            }

            if( System.IO.File.Exists(current_file_name_) )
            {
                saveToFileWithName(current_file_name_);
                Console.WriteLine("Current file saved.");
                stripStatusLabel_.Text = String.Format("Save to {0}",current_file_name_);
                return;
            }
        }

        private void btnUpdatePeriod_Click(object sender, EventArgs e)
        {
            if (period_pattern_.Match(tbCameraPeriod.Text).Success)
            {
                if (listViewCameras.SelectedIndices.Count > 0)
                {
                    int i = listViewCameras.SelectedIndices[0];
                    dcPanel_.UpdateCameraPeriod(i, tbCameraPeriod.Text);
                }
            }
        }

        private void lineGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lineGridToolStripMenuItem.Checked = !lineGridToolStripMenuItem.Checked;
            dcPanel_.SetEnableGrids(lineGridToolStripMenuItem.Checked);
        }

        private void logicGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            logicGridToolStripMenuItem.Checked = !logicGridToolStripMenuItem.Checked;
            dcPanel_.SetEnableLogicWall(logicGridToolStripMenuItem.Checked);
        }
    }
}