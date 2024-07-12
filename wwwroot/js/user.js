var dataTable;
$(document).ready(function () {
  LoadDataTable();
});

function LoadDataTable() {
  dataTable = $('#tblData').DataTable({
    ajax:{ url: '/admin/user/getall'},
    columns: [
    { data: 'fName', width: "25%"},
    { data: 'lName', width: "15%"},
    { data: 'email', width: "25%"},
    { data: 'city', width: "15%"},
    { data: 'district', width: "25%"},
    { data: 'role', width: "25%"},
    {
      data: { id:"id", lockoutEnd:"lockoutEnd"},
      render: function (data) {
        var today = new Date().getTime();
        var lockout = new Date(data.lockoutEnd).getTime();
        if (lockout > today) 
        {
          return "<div class='m-2 text-center'><a onclick=LockUnlock(" + data.id + ") class='btn btn-success text-white'  width:150px;'<i></i> Unlock</a></div><div class='w-75 btn-group' role='group'><a href='/admin/user/Edit?id=" +data+"'class='btn btn-primary mx-2'><i></i>Edit</a><a onClick=Delete('/admin/user/delete?id="+data +"') class='btn btn-danger mx-2'><i><i>Delete</a></div>"
        }
        else
        {
          return "<div class='m-2 text-center'><a onclick=LockUnlock(" + data.id + ") class='btn btn-danger text-white'  width:150px;'<i></i>lock</a></div><div class='w-75 btn-group' role='group'><a href='/admin/user/Edit?id=" +
          data +"'class='btn btn-primary mx-2'><i></i>Edit</a><a onClick=Delete('/admin/user/delete?id=" +data +"') class='btn btn-danger mx-2'><i><i>Delete</a></div>"
        }
      },
      width: "20%",
    },   
  ],
});
}

function LockUnlock(id) {
  $.ajax({
      type: "POST",
      url: '/Admin/User/LockUnlock',
      data: JSON.stringify(id),
      contentType: "application/json",
      success: function (data) {
          if (data.success) {
              toastr.success(data.message);
              dataTable.ajax.reload();
          }
      }
  });
}

function Delete(url) {
  Swal.fire({
    title: "Are you sure?",
    text: "You won't be able to revert this!",
    icon: "warning",
    showCancelButton: true,
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Yes, delete it!",
  }).then((result) => {
    if (result.isConfirmed) {
      $.ajax({
        url: url,
        type: "DELETE",
        success: function (data) {
          dataTable.ajax.reload();
          TransformStream.success(data.message);
        },
      });
    }
  });
}
