using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using B6QLSV.NewFolder1;

namespace B6QLSV
{
    public partial class frmQuanLySV : Form
    {
        private StudentContextDB context = new StudentContextDB();


        public frmQuanLySV()
        {
            InitializeComponent();
        }

        private void frmQuanLySV_Load(object sender, EventArgs e)
        {
            LoadFacultyComboBox();
            LoadStudentDataGridView();
        }

        private void LoadFacultyComboBox()
        {
            var faculties = context.Faculties.ToList();
            cboKhoa.DataSource = faculties;
            cboKhoa.DisplayMember = "FacultyName";
            cboKhoa.ValueMember = "FacultyID";
        }

        private void LoadStudentDataGridView()
        {
            var students = context.Students.Select(s => new
            {
                s.StudentID,
                s.FullName,
                s.AverageScore,
                FacultyName = s.Faculty.FacultyName
            }).ToList();

            dgvSinhVien.DataSource = students;
        }

        private void cboKhoa_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dgvSinhVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = dgvSinhVien.Rows[e.RowIndex];
                txtMaSoSV.Text = row.Cells["StudentID"].Value.ToString();
                txtHoTen.Text = row.Cells["FullName"].Value.ToString();
                txtDiemTB.Text = row.Cells["AverageScore"].Value.ToString();
                cboKhoa.Text = row.Cells["FacultyName"].Value.ToString();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu dữ liệu nhập vào hợp lệ
            if (string.IsNullOrWhiteSpace(txtMaSoSV.Text) ||
                string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                string.IsNullOrWhiteSpace(txtDiemTB.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra và chuyển đổi điểm trung bình
            double averageScore;
            if (!double.TryParse(txtDiemTB.Text, out averageScore))
            {
                MessageBox.Show("Điểm trung bình phải là số hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var newStudent = new Student
                {
                    StudentID = txtMaSoSV.Text,
                    FullName = txtHoTen.Text,
                    AverageScore = averageScore,
                    FacultyID = (int)cboKhoa.SelectedValue
                };

                context.Students.Add(newStudent);
                context.SaveChanges();
                LoadStudentDataGridView();
                MessageBox.Show("Thêm mới sinh viên thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            var studentID = txtMaSoSV.Text;
            var student = context.Students.FirstOrDefault(s => s.StudentID == studentID);

            if (student != null)
            {
                student.FullName = txtHoTen.Text;
                student.AverageScore = double.Parse(txtDiemTB.Text);
                student.FacultyID = (int)cboKhoa.SelectedValue;

                context.SaveChanges();
                LoadStudentDataGridView();
                MessageBox.Show("Cập nhật thông tin sinh viên thành công!");
            }
            else
            {
                MessageBox.Show("Không tìm thấy sinh viên!");
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            var studentID = txtMaSoSV.Text;
            var student = context.Students.FirstOrDefault(s => s.StudentID == studentID);

            if (student != null)
            {
                context.Students.Remove(student);
                context.SaveChanges();
                LoadStudentDataGridView();
                MessageBox.Show("Xóa sinh viên thành công!");
            }
            else
            {
                MessageBox.Show("Không tìm thấy sinh viên!");
            }
        }
    }
}
