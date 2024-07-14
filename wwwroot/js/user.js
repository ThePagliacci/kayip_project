var dataTable;
$(document).ready(function () {
  LoadDataTable();
});

function LoadDataTable() {
  dataTable = $('#tblData').DataTable({
    ajax:{ url: '/admin/user/getall'},
    columns: [
    { data: 'fName', width: "15%"},
    { data: 'lName', width: "15%"},
    { data: 'email', width: "20%"},
    { data: 'city', width: "10%"},
    { data: 'district', width: "10%"},
    { data: 'role', width: "5%"}, 
    {
      data:'id',
      "render": function (data) {
        return  `<div class="w-75 btn-group" role="group">
                     <a href="/admin/user/Edit?id=${data}" class="btn btn-primary mx-2">Edit</a>               
                     <a onClick=Delete('/admin/user/delete/${data}') class="btn btn-danger mx-2">Delete</a>
                    </div>`
      },
      width: "10%",
    },   
    {
      data: { id: "id", lockoutEnd: "lockoutEnd" },
      render: function (data) {
        var today = new Date().getTime();
        var lockout = new Date(data.lockoutEnd).getTime();
        if (lockout > today) {
          return  `<div class="text-center">
                             <a onclick=LockUnLock('${data.id}') class="btn btn-success text-white" style="width:100px;">
                                    <i class="bi bi-lock-fill"></i>  UnLock
                                </a> 
                        </div> `
          } 
          else {
          return `<div class="text-center">
                                <a onclick=LockUnLock('${data.id}') class="btn btn-danger text-white" style="width:100px;">
                                    <i class="bi bi-unlock-fill"></i>  Lock
                                </a>
                    </div>`
       }
      },
      width: "15%",
    }
  ],
});
}

function LockUnLock(id) {
  const token = $('input[name="__RequestVerificationToken"]').val();
  $.ajax({
      type: "POST",
      url: '/Admin/User/LockUnLock',
      data: JSON.stringify(id),
      contentType: "application/json",
      headers: {
        'RequestVerificationToken': token
    },
      success: function (data) {
          if (data.success) {
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
        headers: {
          'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        },
        success: function () {
          dataTable.ajax.reload();
        },
      });
    }
  });
}