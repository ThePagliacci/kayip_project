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
            window.location.reload();
          },
        });
      }
    });
  }

