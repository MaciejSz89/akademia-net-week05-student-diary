using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentDiary
{
    public partial class AddEditGroup : Form
    {
        private int _groupId;
        private Group _group;
        private FileHelper<List<Group>> _fileHelper = new FileHelper<List<Group>>(Program.FilePathGroups);

        public AddEditGroup(int id = 0)
        {
            InitializeComponent();
            _groupId = id;
            GetGroupData();

            tbName.Select();
        }

        private void GetGroupData()
        {
            if (_groupId != 0)
            {
                Text = "Edycja grupy";
                var groups = _fileHelper.DeserializeFromFile();
                _group = groups.FirstOrDefault(x => x.Id == _groupId);

                if (_group == null)
                    throw new Exception("Brak grupy o podanym Id");

                FillTextBoxes();
            }

        }

        private void FillTextBoxes()
        {
            tbId.Text = _group.Id.ToString();
            tbName.Text = _group.Name;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            var groups = _fileHelper.DeserializeFromFile();

            if (_groupId != 0)
                groups.RemoveAll(x => x.Id == _groupId);
            else
                AssignIdToNewGroup(groups);

            AddNewGroupToList(groups);

            _fileHelper.SerializeToFile(groups);


            Close();
        }

        private void AssignIdToNewGroup(List<Group> groups)
        {
            var groupWithHighestId = groups.OrderByDescending(x => x.Id).FirstOrDefault();
            _groupId = groupWithHighestId == null ? 1 : groupWithHighestId.Id + 1;
        }
        private void AddNewGroupToList(List<Group> groups)
        {
            _group = new Group
            {
                Id = _groupId,
                Name = tbName.Text
            };

            groups.Add(_group);
        }

        private void AddEditGroup_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
