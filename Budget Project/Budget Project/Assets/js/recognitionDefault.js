function goBack() {
    const backButton = document.createElement('button');
    backButton.innerHTML = 'Geri Git';
    backButton.style.display = 'none';
    backButton.addEventListener('click', function() {
        window.history.back()
    });
    document.body.appendChild(backButton);
    backButton.click();
}

function goForward() {
    const backButton = document.createElement('button');
    backButton.innerHTML = 'İleri Git';
    backButton.style.display = 'none';
    backButton.addEventListener('click', function() {
        window.history.forward()
    });
    document.body.appendChild(backButton);
    backButton.click();
}

const recognition = new webkitSpeechRecognition();
recognition.lang = 'tr-TR';

document.getElementById('startSpeechRecognition').addEventListener('click', function() {
    recognition.start();
});

recognition.onresult = function(event) {
    const transcript = event.results[0][0].transcript;
    console.log('Tanınan Metin: ', transcript);

    if (transcript.toLowerCase().includes('anasayfayi aç') || transcript.toLowerCase().includes('anasayfa') || transcript.toLowerCase().includes('anasayfaya git')) {
        toastr.success(transcript, "Anasayfa'ya Yönlendiriliyorsunuz");
        setTimeout(function() {
            window.location = '/';
        }, 1500);
    }

    else if (transcript.toLowerCase().includes('geri gel') || transcript.toLowerCase().includes('geri git') || transcript.toLowerCase().includes('geri')) {
        if (window.history.back){
            toastr.success(transcript, "Bir Geri Sayfaya Yönlendiriliyorsunuz");
            setTimeout(function() {
                goBack();
            }, 1500);
        }
        else{
            toastr.warning(transcript, "Geri Dönülecek Sayfa Bulunamadı");
        }
    }

    else if (transcript.toLowerCase().includes('ileri git') || transcript.toLowerCase().includes('ileri')) {
        if (window.history.forward) {
            toastr.success(transcript, "Bir İleri Sayfaya Yönlendiriliyorsunuz");
            setTimeout(function() {
                goForward();
            }, 1500);
        }
        else{
            toastr.warning(transcript, "İleri Sayfa Bulunamadı");
        }
    }

    else if (transcript.toLowerCase().includes('işlemleri aç') || transcript.toLowerCase().includes('işlemleri listele') || transcript.toLowerCase().includes('işlemler') || transcript.toLowerCase().includes('işlemlerim')) {
        toastr.success(transcript, "İşlemler Sayfasına Yönlendiriliyorsunuz");
        setTimeout(function() {
            window.location = '/Transaction';
        }, 1500);
    }

    else if (transcript.toLowerCase().includes('işlemlere git') || transcript.toLowerCase().includes('hesaplarımı aç') || transcript.toLowerCase().includes('hesaplarımı listele') || transcript.toLowerCase().includes('hesaplarım') || transcript.toLowerCase().includes('hesaplar')) {
        toastr.success(transcript, "Hesaplar Sayfasına Yönlendiriliyorsunuz");
        setTimeout(function() {
            window.location = '/Account';
        }, 1500);
    }

    else if (transcript.toLowerCase().includes('hesaplara git') || transcript.toLowerCase().includes('hesaplarımı aç') || transcript.toLowerCase().includes('hesaplarımı listele') || transcript.toLowerCase().includes('hesaplarım') || transcript.toLowerCase().includes('hesaplar')) {
        toastr.success(transcript, "Hesaplar Sayfasına Yönlendiriliyorsunuz");
        setTimeout(function() {
            window.location = '/Account';
        }, 1500);
    }

    else if (transcript.toLowerCase().includes('kategoriler git') || transcript.toLowerCase().includes('kategorileri aç') || transcript.toLowerCase().includes('kategorileri listele') || transcript.toLowerCase().includes('kategoriler') || transcript.toLowerCase().includes('kategorilerim')) {
        toastr.success(transcript, "İşlemler Sayfasına Yönlendiriliyorsunuz");
        setTimeout(function() {
            window.location = '/Category';
        }, 1500);
    }


    else if (transcript.toLowerCase().includes('yeni işlem') || transcript.toLowerCase().includes('işlem oluştur')) {
        toastr.success(transcript, "Yeni İşlem Sayfasına Yönlendiriliyorsunuz");
        setTimeout(function() {
            window.location = '/Transaction/Create';
        }, 1500);
    }

    else if (transcript.toLowerCase().includes('yeni hesap') || transcript.toLowerCase().includes('yeni hesap oluştur') || transcript.toLowerCase().includes('hesap oluştur')) {
        toastr.success(transcript, "Yeni Hesap Sayfasına Yönlendiriliyorsunuz");
        setTimeout(function() {
            window.location = '/Account/Create';
        }, 1500);
    }

    else if (transcript.toLowerCase().includes('yeni kategori') || transcript.toLowerCase().includes('yeni kategori oluştur') || transcript.toLowerCase().includes('kategori oluştur')) {
        toastr.success(transcript, "Yeni Kategori Sayfasına Yönlendiriliyorsunuz");
        setTimeout(function() {
            window.location = '/Category/Create';
        }, 1500);
    }
    else{
        toastr.warning("Sizi Anlayamadık.", "Uyarı!");
    }

};