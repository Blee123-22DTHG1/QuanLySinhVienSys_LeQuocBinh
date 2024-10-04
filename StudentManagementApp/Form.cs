using StudentManagementApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentManagementApp
{
    public partial class frmQuanLySinhVien : System.Windows.Forms.Form
    {
        public frmQuanLySinhVien()
        {
            InitializeComponent();
        }

        private void frmQuanLySinhVien_Load(object sender, EventArgs e)
        {
            using (var context = new StudentContextDB())
            {
                // Load danh sách khoa vào ComboBox
                cbKhoa.DataSource = context.Faculties.ToList();
                cbKhoa.DisplayMember = "FacultyName";
                cbKhoa.ValueMember = "FacultyID";

                // Load danh sách sinh viên vào DataGridView
                var students = from s in context.Students
                               join f in context.Faculties on s.FacultyID equals f.FacultyID
                               select new
                               {
                                   s.StudentID,
                                   s.FullName,
                                   s.AverageScore,
                                   FacultyName = f.FacultyName
                               };
                dgvSinhVien.DataSource = students.ToList();
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            // Kiểm tra thông tin đầu vào
            if (string.IsNullOrEmpty(txtMaSV.Text) || string.IsNullOrEmpty(txtHoTen.Text) || string.IsNullOrEmpty(txtDiemTB.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtMaSV.Text.Length != 10)
            {
                MessageBox.Show("Mã số sinh viên phải có 10 kí tự!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Chuyển đổi txtMaSV.Text sang int
            int studentID;
            if (!int.TryParse(txtMaSV.Text, out studentID))
            {
                MessageBox.Show("Mã số sinh viên không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var context = new StudentContextDB())
            {
                // Tạo mới sinh viên
                var newStudent = new Student
                {
                    StudentID = studentID, // Gán giá trị đã chuyển đổi
                    FullName = txtHoTen.Text,
                    AverageScore = float.Parse(txtDiemTB.Text),
                    FacultyID = (int)cbKhoa.SelectedValue // Đây là kiểu int
                };
                context.Students.Add(newStudent); // Thêm sinh viên vào DbSet
                context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                MessageBox.Show("Thêm mới dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData(); // Gọi hàm để load lại dữ liệu vào DataGridView
            }
        }


        private void btnSua_Click(object sender, EventArgs e)
        {
            using (var context = new StudentContextDB())
            {
                // Chuyển đổi txtMaSV.Text sang kiểu int
                int studentID = int.Parse(txtMaSV.Text);

                // Tìm sinh viên có mã số tương ứng
                var student = context.Students.SingleOrDefault(s => s.StudentID == studentID);
                if (student == null)
                {
                    MessageBox.Show("Không tìm thấy MSSV cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cập nhật thông tin sinh viên
                student.FullName = txtHoTen.Text;
                student.AverageScore = float.Parse(txtDiemTB.Text);
                student.FacultyID = (int)cbKhoa.SelectedValue;

                // Lưu thay đổi vào cơ sở dữ liệu
                context.SaveChanges();

                // Thông báo cập nhật thành công
                MessageBox.Show("Cập nhật dữ liệu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Gọi hàm để load lại dữ liệu vào DataGridView
                LoadData();
            }
        }


        private void btnXoa_Click(object sender, EventArgs e)
        {
            using (var context = new StudentContextDB())
            {
                // Kiểm tra và chuyển đổi txtMaSV.Text sang kiểu int
                int studentID;
                if (!int.TryParse(txtMaSV.Text, out studentID))
                {
                    MessageBox.Show("Mã số sinh viên không hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Tìm sinh viên có mã số tương ứng
                var student = context.Students.SingleOrDefault(s => s.StudentID == studentID);
                if (student == null)
                {
                    MessageBox.Show("Không tìm thấy MSSV cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Hiển thị hộp thoại xác nhận xóa
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    context.Students.Remove(student); // Xóa sinh viên
                    context.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                    MessageBox.Show("Xóa sinh viên thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData(); // Gọi hàm để load lại dữ liệu vào DataGridView
                }
            }
        }


        private void dgvSinhVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvSinhVien.Rows[e.RowIndex];

                // Gán thông tin từ DataGridView vào các điều khiển TextBox và ComboBox
                txtMaSV.Text = row.Cells["StudentID"].Value?.ToString();
                txtHoTen.Text = row.Cells["FullName"].Value?.ToString();
                txtDiemTB.Text = row.Cells["AverageScore"].Value?.ToString();

                // Chuyển đổi FacultyID từ string sang int trước khi gán vào SelectedValue
                if (row.Cells["FacultyID"].Value != null)
                {
                    int facultyID = Convert.ToInt32(row.Cells["FacultyID"].Value);
                    cbKhoa.SelectedValue = facultyID;
                }
            }
        }

        private void LoadData()
        {
            using (var context = new StudentContextDB())
            {
                var students = from s in context.Students
                               join f in context.Faculties on s.FacultyID equals f.FacultyID
                               select new
                               {
                                   s.StudentID,
                                   s.FullName,
                                   s.AverageScore,
                                   FacultyName = f.FacultyName
                               };
                dgvSinhVien.DataSource = students.ToList();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại xác nhận trước khi thoát
            if (MessageBox.Show("Bạn có chắc chắn muốn thoát ứng dụng?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit(); // Đóng ứng dụng
            }
        }

        private void txtMaSV_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
