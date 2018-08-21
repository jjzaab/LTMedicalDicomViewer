using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Leadtools;
using Leadtools.Codecs;
using Leadtools.Dicom;
using Leadtools.MedicalViewer;

namespace MedicalViewerSample
{
    public partial class Form1 : Form
    {
        private MedicalViewer _medicalViewer;

        void MedicalViewerForm_SizeChanged(object sender, EventArgs e)
        {
            _medicalViewer.Size = new Size(this.ClientRectangle.Right, this.ClientRectangle.Bottom);
        }

        public Form1()
        {
            RasterSupport.SetLicense(@"C:\LEADTOOLS 20\Common\License\LEADTOOLS.LIC", System.IO.File.ReadAllText(@"C:\LEADTOOLS 20\Common\License\LEADTOOLS.LIC.KEY"));
            InitializeComponent();
            RasterCodecs _codecs = new RasterCodecs();
            RasterImage _image;
            string dicomFileName = @"C:\Users\Public\Documents\LEADTOOLS Images\IMAGE3.dcm";

            // Create the medical viewer and adjust the size and the location. 
            _medicalViewer = new MedicalViewer(1, 1);
            _medicalViewer.Location = new Point(0, 0);
            _medicalViewer.Size = new Size(this.ClientRectangle.Right, this.ClientRectangle.Bottom);

            // Load an image and then add it to the control. 
            _image = _codecs.Load(dicomFileName);

            DicomEngine.Startup();
            using(DicomDataSet ds = new DicomDataSet())
            {
                MedicalViewerMultiCell cell = new MedicalViewerMultiCell(_image, true, 1, 1);

                ds.Load(dicomFileName, DicomDataSetLoadFlags.None);
                string dpatientID = GetDicomTag(ds, DicomTag.PatientID);
                string dpatientName = GetDicomTag(ds, DicomTag.PatientName);
                string dpatientAge = GetDicomTag(ds, DicomTag.PatientAge);
                string dpatientBirthDate = GetDicomTag(ds, DicomTag.PatientBirthDate);
                string dpatientSex = GetDicomTag(ds, DicomTag.PatientSex);

                // add some actions that will be used to change the properties of the images inside the control. 
                cell.AddAction(MedicalViewerActionType.WindowLevel);
                cell.AddAction(MedicalViewerActionType.Offset);
                cell.AddAction(MedicalViewerActionType.Stack);

                // assign the added actions to a mouse button, meaning that when the user clicks and drags the mouse button, the associated action will be activated. 
                cell.SetAction(MedicalViewerActionType.Offset, MedicalViewerMouseButtons.Right, MedicalViewerActionFlags.Active);
                cell.SetAction(MedicalViewerActionType.WindowLevel, MedicalViewerMouseButtons.Left, MedicalViewerActionFlags.Active);
                cell.SetAction(MedicalViewerActionType.Stack, MedicalViewerMouseButtons.Wheel, MedicalViewerActionFlags.Active);

                // assign the added actions to a keyboard keys that will work like the mouse. 
                MedicalViewerKeys medicalKeys = new MedicalViewerKeys(Keys.Down, Keys.Up, Keys.Left, Keys.Right, MedicalViewerModifiers.None);
                cell.SetActionKeys(MedicalViewerActionType.Offset, medicalKeys);
                medicalKeys.Modifiers = MedicalViewerModifiers.Ctrl;
                cell.SetActionKeys(MedicalViewerActionType.WindowLevel, medicalKeys);
                medicalKeys.MouseDown = Keys.PageDown;
                medicalKeys.MouseUp = Keys.PageUp;
                cell.SetActionKeys(MedicalViewerActionType.Stack, medicalKeys);
                medicalKeys.MouseDown = Keys.Subtract;
                medicalKeys.MouseUp = Keys.Add;
                cell.SetActionKeys(MedicalViewerActionType.Scale, medicalKeys);

                _medicalViewer.Cells.Add(cell);

                // adjust some properties of the cell and add some tags. 
                cell.SetTag(1, MedicalViewerTagAlignment.TopRight, MedicalViewerTagType.UserData, "Name: " + dpatientName);
                cell.SetTag(2, MedicalViewerTagAlignment.TopRight, MedicalViewerTagType.UserData, "ID: " + dpatientID);
                cell.SetTag(3, MedicalViewerTagAlignment.TopRight, MedicalViewerTagType.UserData, "DOB: " + dpatientBirthDate);
                cell.SetTag(4, MedicalViewerTagAlignment.TopRight, MedicalViewerTagType.UserData, "Age: " + dpatientAge);
                cell.SetTag(5, MedicalViewerTagAlignment.TopRight, MedicalViewerTagType.UserData, "Sex: " + dpatientSex);

                cell.SetTag(4, MedicalViewerTagAlignment.TopLeft, MedicalViewerTagType.Frame);
                cell.SetTag(6, MedicalViewerTagAlignment.TopLeft, MedicalViewerTagType.Scale);
                cell.SetTag(2, MedicalViewerTagAlignment.BottomLeft, MedicalViewerTagType.WindowLevelData);
                cell.SetTag(1, MedicalViewerTagAlignment.BottomLeft, MedicalViewerTagType.FieldOfView);
                cell.SetTag(0, MedicalViewerTagAlignment.BottomLeft, MedicalViewerTagType.RulerUnit);

                cell.Rows = 1;
                cell.Columns = 1;
                cell.Frozen = false;
                cell.DisplayRulers = MedicalViewerRulers.Both;
                cell.ApplyOnIndividualSubCell = false;
                cell.ApplyActionOnMove = true;
                cell.FitImageToCell = true;
                cell.Selected = true;
                cell.ShowTags = true;
            }

            string GetDicomTag(DicomDataSet ds, long tag)
            {
                DicomElement patientElement = ds.FindFirstElement(null,
                                                     tag,
                                                     true);

                if (patientElement != null)
                    return ds.GetConvertValue(patientElement);

                return null;
            }
            Controls.Add(_medicalViewer);
            _medicalViewer.Dock = DockStyle.Fill;
        }


    }
}
