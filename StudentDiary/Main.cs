using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace StudentDiary
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            RefreshDiary();

            SetColumnsHeader();

        }

        private FileHelper<List<Student>> _fileHelperStudents = new FileHelper<List<Student>>(Program.FilePathStudents);
        private FileHelper<List<Group>> _fileHelperGroups = new FileHelper<List<Group>>(Program.FilePathGroups);
        private List<Student> _students;
        private List<Group> _groups;
        private IDictionary<int, int> cmbGroupDictionary;

        private void RefreshDiary()
        {
            _students = _fileHelperStudents.DeserializeFromFile();
            _groups = _fileHelperGroups.DeserializeFromFile();

            
            InitializeCmbGroups();

            FillDataGridViewWithActualData();
        }

        private void SetColumnsHeader()
        {
            dgvDiary.Columns[0].HeaderText = "Numer";
            dgvDiary.Columns[1].HeaderText = "Imię";
            dgvDiary.Columns[2].HeaderText = "Nazwisko";
            dgvDiary.Columns[3].HeaderText = "Uwagi";
            dgvDiary.Columns[4].HeaderText = "Matematyka";
            dgvDiary.Columns[5].HeaderText = "Technologia";
            dgvDiary.Columns[6].HeaderText = "Fizyka";
            dgvDiary.Columns[7].HeaderText = "Język polski";
            dgvDiary.Columns[8].HeaderText = "Język obcy";
            dgvDiary.Columns[9].HeaderText = "Zajęcia dodatkowe";
            dgvDiary.Columns[10].HeaderText = "Grupa";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addEditStudent = new AddEditStudent();
            addEditStudent.FormClosing += AddEditStudent_FormClosing;
            addEditStudent.ShowDialog();
        }

        private void AddEditStudent_FormClosing(object sender, FormClosingEventArgs e)
        {
            RefreshDiary();
        }

        private void GroupTable_FormClosing(object sender, FormClosingEventArgs e)
        {
            RefreshDiary();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvDiary.SelectedRows.Count == 0)
            {
                MessageBox.Show("Proszę zaznacz ucznia, którego dane chcesz edytować");
                return;
            }

            var addEditStudent = new AddEditStudent(Convert.ToInt32(dgvDiary.SelectedRows[0].Cells[0].Value));

            addEditStudent.FormClosing += AddEditStudent_FormClosing;
            addEditStudent.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvDiary.SelectedRows.Count == 0)
            {
                MessageBox.Show("Proszę zaznacz ucznia, którego chcesz usunąć");
                return;
            }

            var selectedStudent = dgvDiary.SelectedRows[0];

            var confirmDelete = MessageBox.Show("Usuwanie ucznia", $"Czy na pewno chcesz usunąć ucznia {selectedStudent.Cells[1].Value.ToString().Trim()} {selectedStudent.Cells[2].Value.ToString().Trim()}?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (confirmDelete == DialogResult.OK)
            {

                DeleteStudent(Convert.ToInt32(selectedStudent.Cells[0].Value));
                RefreshDiary();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshDiary();
        }

        private void DeleteStudent(int id)
        {
            var students = _fileHelperStudents.DeserializeFromFile();
            students.RemoveAll(x => x.Id == id);
            _fileHelperStudents.SerializeToFile(students);

        }

        private void btnEditGroups_Click(object sender, EventArgs e)
        {
            var groupTable = new GroupTable();
            groupTable.FormClosing += GroupTable_FormClosing;
            groupTable.ShowDialog();
        }

        private void cmbGroup_SelectedValueChanged(object sender, EventArgs e)
        {
            FillDataGridViewWithActualData();
        }

        private void InitializeCmbGroups()
        {
            
            cmbGroup.Items.Clear();
            cmbGroup.Items.Add("Wszystkie");

            InitializeComboBoxGroupDictionary();

            foreach (var group in _groups)
            {
                cmbGroup.Items.Add(group.Name);
            }
            
            if(cmbGroup.SelectedIndex < 0) 
                cmbGroup.SelectedIndex = 0;
        }

        private void InitializeComboBoxGroupDictionary()
        {
            cmbGroupDictionary = new Dictionary<int, int>();
            int cmbGroupIndex = 1;

            foreach (var group in _groups)
            {
                cmbGroupDictionary.Add(new KeyValuePair<int, int>(cmbGroupIndex, group.Id));
                cmbGroupIndex = cmbGroupIndex + 1;
            }
        }

        private void FillDataGridViewWithActualData()
        {
            if (cmbGroup.SelectedIndex <= 0)
                dgvDiary.DataSource = _students.OrderBy(s => s.Id).Join(_groups, s => s.GroupId, g => g.Id, (s, g) => new { s.Id, s.FirstName, s.LastName, s.Comments, s.Math, s.Technology, s.Physics, s.PolishLang, s.ForeignLang, s.HasOtherActivities, g.Name }).ToList();
            else
            {
                var notFilteredData = _students.OrderBy(s => s.Id).Join(_groups, s => s.GroupId, g => g.Id, (s, g) => new { s.Id, s.FirstName, s.LastName, s.Comments, s.Math, s.Technology, s.Physics, s.PolishLang, s.ForeignLang, s.HasOtherActivities, s.GroupId, g.Name });

                

                dgvDiary.DataSource = notFilteredData.Where( s =>
                {
                    if (!cmbGroupDictionary.TryGetValue(cmbGroup.SelectedIndex, out int selectedGroupId))
                        return false;
                    else
                        return s.GroupId == selectedGroupId;
                }).ToList();
            }
        }

    }
}
