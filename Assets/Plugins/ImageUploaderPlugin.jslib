var ImageUploaderPlugin = {
  OpenImagePicker:function() {
      if (!document.getElementById('ImageUploaderInput')) {
        var fileInput = document.createElement('input');
        fileInput.setAttribute('type', 'file');
        fileInput.setAttribute('id', 'ImageUploaderInput');
        fileInput.setAttribute('accept', '.jpg,.png,.jpeg');
        fileInput.style.visibility = 'hidden';
        fileInput.onclick = function (event) {
          this.value = null;
        };
        fileInput.onchange = function (event) {
          if (event.target.files[0].size > 2097152) {
            alert("Ảnh của bạn có kích thước quá lớn. Vui lòng chọn ảnh nhỏ hơn 2MB.");
            return;
          }
          SendMessage('ChangeAvatarPopup', 'FileSelected', URL.createObjectURL(event.target.files[0]));
        }
        document.body.appendChild(fileInput);
      }
      document.getElementById('ImageUploaderInput').click();
    }
};
mergeInto(LibraryManager.library, ImageUploaderPlugin);