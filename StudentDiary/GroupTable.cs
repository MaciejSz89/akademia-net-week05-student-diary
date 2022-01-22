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
    public partial class GroupTable : Form
    {
        public GroupTable()
        {
            InitializeComponent();

            RefreshGroups();

            SetColumnsHeader();
        }
        private FileHelper<List<Group>> _fileHelper = new FileHelper<List<Group>>(Program.FilePathGroups);

        public void RefreshGroups()
        {
            var groups = _fileHelper.DeserializeFromFile();

            dgvGroups.DataSource = groups.OrderBy(x => x.Id).ToList();
        }

        public void SetColumnsHeader()
        {
            dgvGroups.Columns[0].HeaderText = "Numer";
            dgvGroups.Columns[1].HeaderText = "Nazwa";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var addEditGroup = new AddEditGroup();
            addEditGroup.FormClosing += AddEditGroup_FormClosing;
            addEditGroup.ShowDialog();
        }

        private void AddEditGroup_FormClosing(object sender, FormClosingEventArgs e)
        {
            RefreshGroups();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvGroups.SelectedRows.Count == 0)
            {
                MessageBox.Show("Proszę grupę, którą chcesz edytować");
                return;
            }

            var addEditGroup = new AddEditGroup(Convert.ToInt32(dgvGroups.SelectedRows[0].Cells[0].Value));

            addEditGroup.FormClosing += AddEditGroup_FormClosing;
            addEditGroup.ShowDialog();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvGroups.SelectedRows.Count == 0)
            {
                MessageBox.Show("Proszę zaznacz grupę, którą chcesz usunąć");
                return;
            }

            var selectedGroup = dgvGroups.SelectedRows[0];

            var confirmDelete = MessageBox.Show("Usuwanie grupy", $"Czy na pewno chcesz usunąć grupę {selectedGroup.Cells[1].Value.ToString().Trim()}?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (confirmDelete == DialogResult.OK)
            {

                DeleteGroup(Convert.ToInt32(selectedGroup.Cells[0].Value));
                RefreshGroups();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshGroups();
        }

        private void DeleteGroup(int id)
        {
            var groups = _fileHelper.DeserializeFromFile();
            groups.RemoveAll(x => x.Id == id);
            _fileHelper.SerializeToFile(groups);

        }
    }
}
