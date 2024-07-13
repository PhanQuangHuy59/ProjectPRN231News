$(document).ready(function () {
  function showSuccess(message) {
    toastr.options = {
      closeButton: false,
      debug: false,
      newestOnTop: true,
      progressBar: true,
      positionClass: "toast-top-right",
      preventDuplicates: true,
      onclick: null,
      showDuration: "300",
      hideDuration: "1000",
      timeOut: "5000",
      extendedTimeOut: "1000",
      showEasing: "swing",
      hideEasing: "linear",
      showMethod: "fadeIn",
      hideMethod: "fadeOut",
    };
    toastr["success"](message);
  }

  function showError(message) {
    toastr.options = {
      closeButton: false,
      debug: false,
      newestOnTop: true,
      progressBar: true,
      positionClass: "toast-top-right",
      preventDuplicates: true,
      onclick: null,
      showDuration: "300",
      hideDuration: "1000",
      timeOut: "5000",
      extendedTimeOut: "1000",
      showEasing: "swing",
      hideEasing: "linear",
      showMethod: "fadeIn",
      hideMethod: "fadeOut",
    };
    toastr["error"](message);
  }
  let editor;
  setUpCkeditor("Content");
  function preventAllInput(event) {
    event.preventDefault();
  }

  document.addEventListener("DOMContentLoaded", (event) => {
    var input = document.getElementById("coverimage");
    if (input) {
      input.addEventListener("keydown", preventAllInput);
      input.addEventListener("keypress", preventAllInput);
      input.addEventListener("paste", preventAllInput);
    }
  });

  // xử lý ảnh ở trong nội dung của bài báo
  const submitBtn = document.getElementById("submit");
  const hide_content = document.getElementById("hide_content");
  const title = document.getElementById("Title");

  // Event listener for submit button
  submitBtn.addEventListener("click", async () => {
    // try {
    // Get current time
    const currentTime = new Date();
    const currentTimeString = `${currentTime.getFullYear()}-${
      currentTime.getMonth() + 1
    }-${currentTime.getDate()} ${currentTime.getHours()}:${currentTime.getMinutes()}:${currentTime.getSeconds()}`;

    // Get data from editor and extract images
    const data = editor.getData();
    hide_content.innerHTML = data;
    const imgs = hide_content.querySelectorAll("img");

    const postData = [];

    // Loop through images to collect base64 data
    imgs.forEach((img, index) => {
      const src = img.src;
      if (src.startsWith("data:") && src.includes("base64,")) {
        const startIndex = src.indexOf(":");
        const endIndex = src.indexOf(";");
        const type = src.slice(startIndex + 1, endIndex);

        postData.push({
          name: title.innerText + " " + currentTimeString,
          type: type,
          data: src.split(",")[1],
        });
      }
    });
    // Post data if there are base64 images
    if (postData.length > 0) {
      await postArrayFile(postData);
    } else {
      $("#ClickSave").trigger("click");
      console.log("No base64 images found.");
    }
    // } catch (error) {
    //   showError("Vui lòng thử lại");
    // }
  });

  // Function to post the array of image data
  async function postArrayFile(postData) {
    // try {
    // Show loader
    $("#displayLoader").css("display", "block");

    // Define API endpoint
    var urlApi =
      "https://script.google.com/macros/s/AKfycbyYXFKWB-1vY9pONAjo7l3dcmY4E1bZs3dPyL7dyUgSuXlfGudVekbTdrPZnzAIuWTx/exec";

    const response = await fetch(urlApi, {
      method: "POST",
      body: JSON.stringify(postData),
    });

    // Check response status
    if (!response.ok) {
      $("#displayLoader").css("display", "none");
      throw new Error(`HTTP error! Status: ${response.status}`);
    }
    // Parse response data
    const data = await response.json();
    console.log(data);
    // Update image sources in the editor
    const imgs = hide_content.querySelectorAll("img");
    imgs.forEach((img, index) => {
      const src = img.src;
      if (src.startsWith("data:") && src.includes("base64,")) {
        console.log(data[0].link);
        img.src = data.shift().link; // Update src and remove first element from data array
      }
    });

    // Update editor content with new image sources
    const content = hide_content.innerHTML;
    debugger;
    editor.setData(content);
    $("#displayLoader").css("display", "none");
    $("#ClickSave").trigger("click");
    // } catch (error) {
    //   console.error("Error:", error);
    //   showError("Vui lòng thử lại");
    // } finally {
    //   // Hide loader
    //   $("#displayLoader").css("display", "none");
    // }
  }

  //UpLoad Hình ảnh

  const elInput = document.getElementById("inputCovorIamge");
  const img = document.getElementById("imgPreViewLoad");
  const uploadBtn = document.getElementById("clickLoadCoverImage");
  const coverImg = $("#coverimage");
  uploadBtn.addEventListener("click", () => {
    elInput.click();
  });

  elInput.addEventListener("change", previewSelectedImage);
  function previewSelectedImage() {
    const file = elInput.files[0];

    const currentTime = new Date();
    const year = currentTime.getFullYear();
    const month = currentTime.getMonth() + 1;
    const day = currentTime.getDate();
    const hour = currentTime.getHours();
    const minute = currentTime.getMinutes();
    const second = currentTime.getSeconds();

    const currentTimeString = `${year}-${month} -${day} ${hour}:${minute}:${second}`;
    if (file) {
      const reader = new FileReader();
      reader.readAsDataURL(file);

      reader.addEventListener("load", (e) => {
        img.src = e.target.result;
        const data = reader.result.split(",")[1];
        const postData = {
          name: currentTimeString + file.name,
          type: file.type,
          data: data,
        };
        console.log(postData);
        postFile(postData);
      });
    }
  }

  async function postFile(postData) {
    var apiAppScript =
      "https://script.google.com/macros/s/AKfycbxbO6hUsYNYIEx_ZMY7MvxmT0dFo7zkE5nbg61pexLVVbc2cuicOSHLfHbysv2VxEmz/exec";

    $("#displayLoader").css("display", "block");
    const response = await fetch(apiAppScript, {
      method: "POST",
      body: JSON.stringify(postData),
    });
    if (response.ok) {
      const data = await response.json();
      console.log(data);
      img.src = data.link;
      coverImg.val(data.link);
      $("#displayLoader").css("display", "none");
      showSuccess("Đã chọn ảnh cho bài báo thành công.");
    }
  }

  function setUpCkeditor(id) {
    CKEDITOR.ClassicEditor.create(document.getElementById(id), {
      // https://ckeditor.com/docs/ckeditor5/latest/features/toolbar/toolbar.html#extended-toolbar-configuration-format
      toolbar: {
        items: [
          "exportPDF",
          "exportWord",
          "|",
          "findAndReplace",
          "selectAll",
          "|",
          "heading",
          "|",
          "bold",
          "italic",
          "strikethrough",
          "underline",
          "code",
          "subscript",
          "superscript",
          "removeFormat",
          "|",
          "bulletedList",
          "numberedList",
          "todoList",
          "|",
          "outdent",
          "indent",
          "|",
          "undo",
          "redo",
          "-",
          "fontSize",
          "fontFamily",
          "fontColor",
          "fontBackgroundColor",
          "highlight",
          "|",
          "alignment",
          "|",
          "link",
          "uploadImage",
          "blockQuote",
          "insertTable",
          "mediaEmbed",
          "codeBlock",
          "htmlEmbed",
          "|",
          "specialCharacters",
          "horizontalLine",
          "pageBreak",
          "|",
          "textPartLanguage",
          "|",
          "sourceEditing",
        ],
        shouldNotGroupWhenFull: true,
      },
      // Changing the language of the interface requires loading the language file using the <script> tag.
      // language: 'es',
      list: {
        properties: {
          styles: true,
          startIndex: true,
          reversed: true,
        },
      },
      // https://ckeditor.com/docs/ckeditor5/latest/features/headings.html#configuration
      heading: {
        options: [
          {
            model: "paragraph",
            title: "Paragraph",
            class: "ck-heading_paragraph",
          },
          {
            model: "heading1",
            view: "h1",
            title: "Heading 1",
            class: "ck-heading_heading1",
          },
          {
            model: "heading2",
            view: "h2",
            title: "Heading 2",
            class: "ck-heading_heading2",
          },
          {
            model: "heading3",
            view: "h3",
            title: "Heading 3",
            class: "ck-heading_heading3",
          },
          {
            model: "heading4",
            view: "h4",
            title: "Heading 4",
            class: "ck-heading_heading4",
          },
          {
            model: "heading5",
            view: "h5",
            title: "Heading 5",
            class: "ck-heading_heading5",
          },
          {
            model: "heading6",
            view: "h6",
            title: "Heading 6",
            class: "ck-heading_heading6",
          },
        ],
      },
      // https://ckeditor.com/docs/ckeditor5/latest/features/editor-placeholder.html#using-the-editor-configuration
      placeholder: "Hãy nhập nội dung vào đây.",
      // https://ckeditor.com/docs/ckeditor5/latest/features/font.html#configuring-the-font-family-feature
      fontFamily: {
        options: [
          "default",
          "Arial, Helvetica, sans-serif",
          "Courier New, Courier, monospace",
          "Georgia, serif",
          "Lucida Sans Unicode, Lucida Grande, sans-serif",
          "Tahoma, Geneva, sans-serif",
          "Times New Roman, Times, serif",
          "Trebuchet MS, Helvetica, sans-serif",
          "Verdana, Geneva, sans-serif",
        ],
        supportAllValues: true,
      },
      // https://ckeditor.com/docs/ckeditor5/latest/features/font.html#configuring-the-font-size-feature
      fontSize: {
        options: [10, 12, 14, "default", 18, 20, 22],
        supportAllValues: true,
      },
      // Be careful with the setting below. It instructs CKEditor to accept ALL HTML markup.
      // https://ckeditor.com/docs/ckeditor5/latest/features/general-html-support.html#enabling-all-html-features
      htmlSupport: {
        allow: [
          {
            name: /.*/,
            attributes: true,
            classes: true,
            styles: true,
          },
        ],
      },
      // Be careful with enabling previews
      // https://ckeditor.com/docs/ckeditor5/latest/features/html-embed.html#content-previews
      htmlEmbed: {
        showPreviews: true,
      },
      // https://ckeditor.com/docs/ckeditor5/latest/features/link.html#custom-link-attributes-decorators
      link: {
        decorators: {
          addTargetToExternalLinks: true,
          defaultProtocol: "https://",
          toggleDownloadable: {
            mode: "manual",
            label: "Downloadable",
            attributes: {
              download: "file",
            },
          },
        },
      },
      // https://ckeditor.com/docs/ckeditor5/latest/features/mentions.html#configuration
      mention: {
        feeds: [
          {
            marker: "@",
            feed: [
              "@apple",
              "@bears",
              "@brownie",
              "@cake",
              "@cake",
              "@candy",
              "@canes",
              "@chocolate",
              "@cookie",
              "@cotton",
              "@cream",
              "@cupcake",
              "@danish",
              "@donut",
              "@dragée",
              "@fruitcake",
              "@gingerbread",
              "@gummi",
              "@ice",
              "@jelly-o",
              "@liquorice",
              "@macaroon",
              "@marzipan",
              "@oat",
              "@pie",
              "@plum",
              "@pudding",
              "@sesame",
              "@snaps",
              "@soufflé",
              "@sugar",
              "@sweet",
              "@topping",
              "@wafer",
            ],
            minimumCharacters: 1,
          },
        ],
      },
      // The "superbuild" contains more premium features that require additional configuration, disable them below.
      // Do not turn them on unless you read the documentation and know how to configure them and setup the editor.
      removePlugins: [
        // These two are commercial, but you can try them out without registering to a trial.
        // 'ExportPdf',
        // 'ExportWord',
        "AIAssistant",
        "CKBox",
        "CKFinder",
        "EasyImage",
        // This sample uses the Base64UploadAdapter to handle image uploads as it requires no configuration.
        // https://ckeditor.com/docs/ckeditor5/latest/features/images/image-upload/base64-upload-adapter.html
        // Storing images as Base64 is usually a very bad idea.
        // Replace it on production website with other solutions:
        // https://ckeditor.com/docs/ckeditor5/latest/features/images/image-upload/image-upload.html
        // "Base64UploadAdapter",
        "MultiLevelList",
        "RealTimeCollaborativeComments",
        "RealTimeCollaborativeTrackChanges",
        "RealTimeCollaborativeRevisionHistory",
        "PresenceList",
        "Comments",
        "TrackChanges",
        "TrackChangesData",
        "RevisionHistory",
        "Pagination",
        "WProofreader",
        // Careful, with the Mathtype plugin CKEditor will not load when loading this sample
        // from a local file system (file://) - load this site via HTTP server if you enable MathType.
        "MathType",
        // The following features are part of the Productivity Pack and require additional license.
        "SlashCommand",
        "Template",
        "DocumentOutline",
        "FormatPainter",
        "TableOfContents",
        "PasteFromOfficeEnhanced",
        "CaseChange",
      ],
    })
      .then((newEditor) => {
        editor = newEditor;
      })
      .catch((error) => {
        console.error(error);
      });
  }
});
