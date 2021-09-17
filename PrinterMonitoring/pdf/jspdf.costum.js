function pdftext(text) {
        return text.split(' ').join('&nbsp')
    }

    function qrBarcode(element, text) {
        const qrcode = new QRCode(element, {
            width: 150,
            height: 150,
            colorDark: "#000000",
            colorLight: "#ffffff",
            correctLevel: QRCode.CorrectLevel.H
        });
        qrcode.makeCode(text);
    }

    function generatePDF({
        datas: datas, // array,
        autoPrint: autoPrint, // boolean -- default false
        save: save, // boolean -- default false
        target: target // 'THIS_TAB' or 'NEW_TAB' -- default 'NEW_TAB'
    }) {
		return new Promise((resolve, reject) => {
			const pdf = new jsPDF('p', 'px', 'a4');
        //console.log(pdf.internal.pageSize.getHeight()
        let pdf_main = document.createElement("div");
        pdf_main.setAttribute("id", "pdf-main");

        let pdfRows = Math.ceil(datas.length / 3)

        let contentIndex = 0;
        for (let i = 1; i <= pdfRows; i++) {
            let pdf_row = document.createElement("div");
            pdf_row.setAttribute("class", "pdf-row");
            pdf_main.appendChild(pdf_row)
            for (let ii = 1; ii <= 3; ii++) {
                if (datas[contentIndex]) {
                    let pdf_content = document.createElement("div");
                    pdf_content.setAttribute("class", "pdf-content");
                    pdf_row.appendChild(pdf_content)
                    let pdf_margin = document.createElement("div");
                    pdf_margin.setAttribute("class", "pdf-margin");
                    pdf_content.appendChild(pdf_margin)
                    let pdf_qrcode = document.createElement("div");
                    pdf_qrcode.setAttribute("class", "pdf-qrcode");
                    pdf_margin.appendChild(pdf_qrcode)
                    let pdf_title = document.createElement("div");
                    pdf_title.setAttribute("class", "pdf-title");
                    pdf_margin.appendChild(pdf_title)
                    pdf_title.innerHTML = pdftext(datas[contentIndex])
                    qrBarcode(pdf_qrcode, String(datas[contentIndex]))
                }
                contentIndex++;
            }
        }
        

        pdf.html(pdf_main, {
            html2canvas: {
                // insert html2canvas options here, e.g.
                //width: 50
                scale: 0.5,
            },
            callback: function (context) {
                (save === true) ? pdf.save('inventory.pdf') : null;
                (autoPrint === true) ? pdf.autoPrint() : null;
                (target === 'THIS_TAB' || target === 'NEW_TAB') ? (target === 'THIS_TAB') ? window.open(pdf.output('bloburl'), '_self') : window.open(pdf.output('bloburl'), '_blank') : null;
				resolve()
            }
        })
		})
    }
