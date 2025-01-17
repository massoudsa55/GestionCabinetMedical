﻿using DevExpress.XtraEditors;
using Gestion_Cabinet_Medical.Functions;
using System;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Gestion_Cabinet_Medical.Forms.Consultation
{
    public partial class Nouvelle_Consultation : XtraForm
    {
        public int _ID_Patient;
        int _ID_Motif;
        public int _ID_Consultation;
        public string EditOrAdd;
        DAL.Patient patient;
        DAL.Consultations consultations;
        DAL.Antecedents antecedents;
        public string _libelleMotif = "";
        public Nouvelle_Consultation()
        {
            InitializeComponent();
        }

        private void Nouvelle_Consultation_Load(object sender, EventArgs e)
        {
            switch (EditOrAdd)
            {
                case "New":
                    LoadData();
                    break;
                case "Edit":
                    LaodDataForEdit(_ID_Consultation);
                    break;
                default:
                    break;
            }
            pic_AddMotifs.Click += Pic_AddMotifs_Click;
            txt_Taille.KeyUp += Txt_Taille_KeyUp;
            btn_CalcIMC.Click += Btn_CalcIMC_Click;
            btn_Valid.Click += Btn_Valid_Click;
            btn_Annuler.Click += Btn_Annuler_Click;
            #region Evants TextFocused
            txt_Poids.GotFocus += TextEdit_GotFocus;
            txt_Taille.GotFocus += TextEdit_GotFocus;
            txt_Temperator.GotFocus += TextEdit_GotFocus;
            txt_FCardiaque.GotFocus += TextEdit_GotFocus;
            txt_Glycemie.GotFocus += TextEdit_GotFocus;
            txt_PressionArterielle.GotFocus += TextEdit_GotFocus;
            #endregion
        }

        private void Btn_Annuler_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Btn_Valid_Click(object sender, EventArgs e)
        {
            if (!IsDataValide())
                return;
            switch (EditOrAdd)
            {
                case "New":
                    Save();
                    break;
                case "Edit":
                    Edit();
                    break;
                default:
                    break;
            }
            Close();
        }

        public void Edit()
        {
            SetDataConsultation();
            SetDataAntecedent();
            Master.db.Entry(consultations).State = EntityState.Modified;
            Master.db.Entry(antecedents).State = EntityState.Modified;
            Master.db.SaveChanges();
            XtraMessageBox.Show("Edited Succesffuly", "Edit", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Btn_CalcIMC_Click(object sender, EventArgs e)
        {
            calcIMC();
        }

        private void Txt_Taille_KeyUp(object sender, KeyEventArgs e)
        {
            calcIMC();
        }

        private void TextEdit_GotFocus(object sender, EventArgs e)
        {
            if (sender is TextEdit sendTXT)
            {
                switch (sendTXT.Name)
                {
                    case "txt_Poids":
                        ClearTextFromBox(txt_Poids);
                        break;
                    case "txt_Taille":
                        ClearTextFromBox(txt_Taille);
                        break;
                    case "txt_Temperator":
                        ClearTextFromBox(txt_Temperator);
                        break;
                    case "txt_FCardiaque":
                        ClearTextFromBox(txt_FCardiaque);
                        break;
                    case "txt_Glycemie":
                        ClearTextFromBox(txt_Glycemie);
                        break;
                    case "txt_PressionArterielle":
                        ClearTextFromBox(txt_PressionArterielle);
                        break;
                    default:
                        break;
                }
            }
        }

        private void Pic_AddMotifs_Click(object sender, EventArgs e)
        {
            Nouveau_Motif nouveau_Motif = new Nouveau_Motif();
            nouveau_Motif.ShowDialog();
            _libelleMotif = nouveau_Motif.txt_LibelleMotif.Text;
            lookUpEdit1.CustomDisplayText += LookUpEdit1_CustomDisplayText;
            LoadData();
        }

        private void LookUpEdit1_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            if (_libelleMotif != string.Empty)
                e.DisplayText = string.Concat(_libelleMotif);
        }

        public void LoadData()
        {
            var query = from motif in Master.db.Motifs
                        select new
                        {
                            ID = motif.ID_Motifs,
                            motif.Libelle,
                            motif.Adreviation
                        };
            patient = Master.db.Patient.First(a => a.ID_Patient == _ID_Patient);
            if (patient == null)
                return;
            dateEdit1.DateTime = DateTime.Now.Date;
            lookUpEdit1.Properties.DataSource = query.ToList();
            lookUpEdit1.Properties.ValueMember = "ID";
            lookUpEdit1.Properties.DisplayMember = nameof(DAL.Motifs.Libelle);
            label_NomPrenom.Text = patient.Nom + " " + patient.Prenom;
            if (patient.Image != null)
                pic_ImagePatient.Image = Master.GetImageFromByteArray(patient.Image);
            label_NumbreConsultation.Text = Master.db.Consultations.Count(a => a.ID_Patient == _ID_Patient).ToString();
            if (Master.db.Attende.Any(a => a.ID_Patient == _ID_Patient))
                label_SalleAttente.Text = "Oui";
            else
                label_SalleAttente.Text = "Non";
        }

        public string GetMotifs(int ID_Motif)
        {
            var motif = Master.db.Motifs.First(a => a.ID_Motifs == ID_Motif).Libelle.ToString();
            return motif != string.Empty ? motif : "";
        }
        public void LaodDataForEdit(int ID_Consult)
        {
            LoadData();
            GetConsultation(ID_Consult);
            GetAntecedent(ID_Consult);
        }

        public void GetAntecedent(int ID_Consult)
        {
            if (!Master.db.Consultations.Any(a => a.ID_Consultation == ID_Consult))
                return;
            consultations = Master.db.Consultations.First(a => a.ID_Consultation == ID_Consult);
            lookUpEdit1.Text = GetMotifs((int)consultations.ID_Motifs);
            dateEdit1.DateTime = consultations.DateTime.Date;
            txt_Poids.Text = consultations.Poids.ToString();
            txt_Taille.Text = consultations.Taille.ToString();
            txt_Temperator.Text = consultations.Temperature.ToString();
            txt_FCardiaque.Text = consultations.FrequenceCardiaque.ToString();
            txt_Glycemie.Text = consultations.Glycecmie.ToString();
            txt_PressionArterielle.Text = consultations.PressionArterielle.ToString();
            me_Note.Text = (consultations.Note == null) ? "" : consultations.Note.ToString();
        }

        public void GetConsultation(int ID_Consult)
        {
            if (!Master.db.Antecedents.Any(a => a.ID_Consultation == ID_Consult))
                return;
            antecedents = Master.db.Antecedents.First(a => a.ID_Consultation == ID_Consult);
            me_Anti_Medicaux.Text = antecedents.Anti_Medicaux.ToString();
            me_Anti_Chirurgicaux.Text = antecedents.Anti_Chirurgicaux.ToString();
            me_Anti_Familiale.Text = antecedents.Anti_Familiales.ToString();
            me_Anti_Autre.Text = antecedents.Autres_Anti.ToString();
        }
        public bool TextBoxIsNotDigit(TextEdit textEdit) => System.Text.RegularExpressions.Regex.IsMatch(textEdit.Text, "[^0-9]");

        public void calcIMC()
        {
            if (TextBoxIsNotDigit(txt_Poids) || TextBoxIsNotDigit(txt_Taille))
                return;
            if ((txt_Poids.Text == string.Empty || txt_Taille.Text == string.Empty))
                return;
            double tailleMitre = double.Parse(txt_Taille.Text) / 100;
            double IMC = int.Parse(txt_Poids.Text) / (tailleMitre * tailleMitre);
            label_IMC_Value.Text = IMC.ToString("0.0");
            if (IMC < 18.5)
            {
                // no9s lwazen
                // Insuffisance pondérale
                label_IMC_Status.Text = "Insuffisance pondérale";
                label_IMC_Status.BackColor = Color.Blue;
            }
            else
            {
                if ((IMC >= 18.5) && (IMC <= 24.9))
                {
                    // naturelle
                    // Poids normal
                    label_IMC_Status.Text = "Poids normal";
                    label_IMC_Status.BackColor = Color.FromArgb(128, 255, 128);
                }
                else
                {
                    if ((IMC >= 25) && (IMC <= 29.9))
                    {
                        // wazen zaaid
                        // Surpoids
                        label_IMC_Status.Text = "Surpoids";
                        label_IMC_Status.BackColor = Color.Orange;
                    }
                    else
                    {
                        if (IMC >= 30)
                        {
                            // somna
                            // Obésité
                            label_IMC_Status.Text = "Obésité";
                            label_IMC_Status.BackColor = Color.Red;
                        }
                    }
                }
            }
        }

        public void ClearTextFromBox(TextEdit text) => text.Text = string.Empty;

        public void GetIdMotifs()
        {
            if (lookUpEdit1.EditValue != null && lookUpEdit1.Text != string.Empty)
            {
                var id_Motif = (int?)Master.db.Motifs.First(a => a.Libelle == lookUpEdit1.Text).ID_Motifs ?? 0;
                if (id_Motif > 0)
                    _ID_Motif = id_Motif;
            }
        }

        public void GetIdConsultations()
        {
            var idConsult = Master.db.Consultations.Select(a => a.ID_Consultation).Max();
            _ID_Consultation = (int?)idConsult ?? 1;
        }

        public void SetDataConsultation()
        {
            GetIdMotifs();
            if (EditOrAdd == "New")
                consultations = new DAL.Consultations
                {
                    ID_Patient = _ID_Patient
                };
            consultations.DateTime = dateEdit1.DateTime.Date;
            consultations.ID_Motifs = _ID_Motif;
            consultations.Poids = TextBoxIsNotDigit(txt_Poids) ? 0 : int.Parse(txt_Poids.Text);
            consultations.Taille = TextBoxIsNotDigit(txt_Taille) ? 0 : int.Parse(txt_Taille.Text);
            consultations.Temperature = TextBoxIsNotDigit(txt_Temperator) ? 0 : int.Parse(txt_Temperator.Text);
            consultations.FrequenceCardiaque = TextBoxIsNotDigit(txt_FCardiaque) ? 0 : int.Parse(txt_FCardiaque.Text);
            consultations.Glycecmie = txt_Glycemie.Text;
            consultations.PressionArterielle = txt_PressionArterielle.Text;
            consultations.Note = me_Note.Text;
        }

        public void SetDataAntecedent()
        {
            GetIdConsultations();
            if (EditOrAdd == "New")
                antecedents = new DAL.Antecedents();
            antecedents.ID_Consultation = _ID_Consultation;
            antecedents.Anti_Medicaux = me_Anti_Medicaux.Text;
            antecedents.Anti_Chirurgicaux = me_Anti_Chirurgicaux.Text;
            antecedents.Anti_Familiales = me_Anti_Familiale.Text;
            antecedents.Autres_Anti = me_Anti_Autre.Text;
        }
        public void Save()
        {
            SetDataConsultation();
            Master.db.Consultations.Add(consultations);
            Master.db.SaveChanges();
            SetDataAntecedent();
            Master.db.Antecedents.Add(antecedents);
            Master.db.SaveChanges();
            XtraMessageBox.Show("Saved Succesffuly", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //NumberOfErrors += Master.IsEditValueValideAndNotZero(loo)
        public bool IsDataValide()
        {
            int NumberOfErrors = 0;
            NumberOfErrors += Master.IsEditValueValideAndNotZero(lookUpEdit1) ? 0 : 1;
            return (NumberOfErrors == 0);
        }

    }
}
