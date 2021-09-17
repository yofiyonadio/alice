//const qrcode = window.qrcode;

const video = document.createElement("video");
const canvasElement = document.getElementById("qr-canvas");
const canvas = canvasElement.getContext("2d");

let scanning = false;


qrcode.callback = res => {
    if (res) {
        barcodeCamEnd().then(() => {
            barcodeCallback(res)
        })
    /*
    setTimeout(function () {
        scan();
    }, 1000);
    */
  }
};

function barcodeCamEnd() {
    return new Promise((resolve, reject) => {
         $("#barcode-wrapper").css({ 'background-color': 'rgba(255, 255, 255, 1)' });
         scanning = false
         if (video.srcObject !== null) {
             video.srcObject.getTracks().forEach(track => {
                 track.stop();
             });
         }
         
         resolve()
    })
}

function openBarcodecam() {
    return new Promise((resolve, reject) => {
        navigator.mediaDevices
    .getUserMedia({ video: { facingMode: "environment" } })
    .then(function (stream) {
        scanning = true;
        canvasElement.hidden = false;
        video.setAttribute("playsinline", true); // required to tell iOS safari we don't want fullscreen
        video.srcObject = stream;
        video.play();
        tick();
        scan();
        resolve()
    });
    })
}

function tick() {
  canvasElement.height = video.videoHeight;
  canvasElement.width = video.videoWidth;
  canvas.drawImage(video, 0, 0, canvasElement.width, canvasElement.height);

  scanning && requestAnimationFrame(tick);
}

function scan() {
  try {
    qrcode.decode();
  } catch (e) {
    setTimeout(scan, 300);
  }
}
