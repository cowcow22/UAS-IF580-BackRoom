# UAS-IF580-BackRoom

Ketiga File Dibawah ini dapat diakses melalui link Google Drive pada laporan

1. Untuk File The BlackJek.zip didalamnya terdapat file The BlackJek.exe untuk menjalankan Aplikasi Gamenya
2. Untuk File UAS-IF590-BackRoom.zip didalamnya berisi file, code, dan asset yang digunakan untuk game ini
3. Untuk File TheBlackJek.unitypackage berisi file, code dan asset yang digunakan untuk game ini. Untuk menggunakan file ini silahkan gunakan Universal 3D untuk pembuatan project awal di Unityhub

Jika ada eror terkait seperti dibawah ini:
Assets\GenerationManager.cs(7,13): error CS0234: The type or namespace name 'AI' does not exist in the namespace 'Unity' (are you missing an assembly reference?)
Assets\GenerationManager.cs(32,22): error CS0246: The type or namespace name 'NavMeshSurface' could not be found (are you missing a using directive or an assembly reference?)

Lakukan langkah-langkah berikut untuk memperbaiki:
1. Install AI Navigation Package: Buka Unity dan ke Window > Package Manager.
2. Di Package Manager, pilih Unity Registry di dropdown menu di kiri atas.
3. Cari AI Navigation package.
4. Install package jika belum ter install. Package ini termasuk NavMeshSurface dan komponen lainnya.
