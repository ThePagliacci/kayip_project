var dataTable;
$(document).ready(function () {
  LoadDataTable();
});

function LoadDataTable() {
  dataTable = $('#tblData').DataTable({
    ajax:{ url: '/admin/post/getall'},
    columns: [
    { data: 'title', width: "15%"}, 
    { data: 'description', width: "15%"},
    { data: 'contactInfo', width: "15%"},
    { data: 'image',
    "render": function (data) {
      var correctedUrl = data.startsWith('/') ? data : '/' + data;
      return `<img style="width:50%;"src="${correctedUrl}"/>`;
    },
      width: "25%"},
    { data: 'applicationUser.fName', width: "15%"},
    {
      data: 'id',
      render: function (data) {
        return `<div class="w-75 btn-group" role="group"><a href="/admin/post/upsert?id=${data}" class="btn btn-primary mx-2">Edit</a><a onClick=Delete('/admin/post/delete/${data}') class="btn btn-danger mx-2">Delete</a></div>`
      },
      width: "15%",
    },
  ],
});
}
function Delete(url) {
  Swal.fire({
    title: "Emin misiniz?",
    text: "Bunu geri döndüremezsiniz!",
    icon: "warning",
    showCancelButton: true,
    confirmButtonColor: "#3085d6",
    cancelButtonColor: "#d33",
    confirmButtonText: "Evet, kaldır!",
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