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
    render: function (data) {
      return '<img src="' + data + '" style="width:100%;" />';
    },
      width: "25%"},
    { data: 'applicationUser', width: "15%"},
    {
      data: 'id',
      render: function (data) {
        return (
          "<div class='w-75 btn-group' role='group'><a href='/admin/post/upsert?id=" +
          data +
          "'class='btn btn-primary mx-2'><i class='bi bi-pencil-square'></i>Edit</a><a onClick=Delete('/admin/post/delete?id=" +
          data +
          "') class='btn btn-danger mx-2'><i class='bi bi-trash-fill'><i>Delete</a></div>"
        );
      },
      width: "15%",
    },
  ],
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
