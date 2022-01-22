using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace StudentDiary
{
    public partial class AddEditStudent : Form
    {
        public AddEditStudent(int id = 0)
        {
            InitializeComponent();
            _studentId = id;
            GetStudentData();

            tbFirstName.Select();
            
        }

        private int _studentId;
        private Student _student;
        private List<Group> _groups;
        private IDictionary<int, int> cmbGroupDictionary;
        private FileHelper<List<Student>> _fileHelperStudents = new FileHelper<List<Student>>(Program.FilePathStudents);
        private FileHelper<List<Group>> _fileHelperGroups = new FileHelper<List<Group>>(Program.FilePathGroups);


        private void GetStudentData()
        {
            if (_studentId != 0)
            {
                Text = "Edycja danych ucznia";
                var students = _fileHelperStudents.DeserializeFromFile();
                _groups = _fileHelperGroups.DeserializeFromFile();
                _student = students.FirstOrDefault(x => x.Id == _studentId);

                if (_student == null)
                    throw new Exception("Brak użytkownika o podanym Id");

                FillTextBoxes();

                
                InitializeCmbGroups();
            }
            
        }

        private void FillTextBoxes()
        {
            tbId.Text = _student.Id.ToString();
            tbFirstName.Text = _student.FirstName;
            tbLastName.Text = _student.LastName;
            tbMath.Text = _student.Math;
            tbTechnology.Text = _student.Technology;
            tbPhysics.Text = _student.Physics;
            tbPolishLang.Text = _student.PolishLang;
            tbForeignLang.Text = _student.ForeignLang;
            rtbComments.Text = _student.Comments;
            chbHasOtherActiviities.Checked = _student.HasOtherActivities;        

        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            var students = _fileHelperStudents.DeserializeFromFile();

            if (_studentId != 0)
                students.RemoveAll(x => x.Id == _studentId);
            else
                AssignIdToNewStudent(students);
           
            AddNewStudentToList(students);

            _fileHelperStudents.SerializeToFile(students);


            Close();
        }

        private void AssignIdToNewStudent(List<Student> students)
        {
            var studentWithHighestId = students.OrderByDescending(x => x.Id).FirstOrDefault();
            _studentId = studentWithHighestId == null ? 1 : studentWithHighestId.Id + 1;
        }

        private void AddNewStudentToList(List<Student> students)
        {
            _student = new Student
            {
                Id = _studentId,
                FirstName = tbFirstName.Text,
                LastName = tbLastName.Text,
                Math = tbMath.Text,
                Technology = tbTechnology.Text,
                Physics = tbPhysics.Text,
                PolishLang = tbPolishLang.Text,
                ForeignLang = tbForeignLang.Text,
                Comments = rtbComments.Text,
                HasOtherActivities = chbHasOtherActiviities.Checked,
                GroupId = _groups != null ? _groups.Where(x => cmbGroup.SelectedItem.ToString() == x.Name).FirstOrDefault().Id : 0
            };

            students.Add(_student);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }


        private void cmbGroup_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cmbGroupDictionary.TryGetValue(cmbGroup.SelectedIndex, out int groupId))
                 _student.GroupId = groupId;

        }

        private void InitializeCmbGroups()
        {
            InitializeComboBoxGroupDictionary();
            
            foreach (var group in _groups)
            {
                cmbGroup.Items.Add(group.Name);
            }
            cmbGroup.SelectedIndex = cmbGroupDictionary.Where(x => x.Value == _student.GroupId).Select(x => x.Key).FirstOrDefault();

            if (cmbGroup.SelectedIndex < 0)
                cmbGroup.SelectedIndex = 0;
        }

        private void InitializeComboBoxGroupDictionary()
        {
            cmbGroupDictionary = new Dictionary<int, int>();
            int cmbGroupIndex = 0;

            foreach (var group in _groups)
            {
                cmbGroupDictionary.Add(new KeyValuePair<int, int>(cmbGroupIndex, group.Id));
                cmbGroupIndex = cmbGroupIndex + 1;
            }
        }
    }
}
