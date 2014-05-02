Passos para gerar uma vers�o release.
Antes de gerar a vers�o conferir se precisa aumentar a vers�o do app nas suas configura��es.
N�O fazer commit da vers�o lite. Ou seja, se vc rodar o script para vers�o lite, n�o fazer o commit. Isso n�o � regra, � um conven��o. Afinal vc pode desenvolver com a Lite s� usando as configura��es de build no Xamarin Studio.


Pr�-requisitos:
1- Inkscape 0.48 instalado
2- Python instalado e diret�rio no Path. (uso o Python 3.3)

Build Yester Full
1- Gerar as imagens:
   - Abrir o leitor dos scripts. Pasta Glyphs\InkscapeBatch\InkscapeBatchGUI.Exe
   - Abrir o script em Glyphs\theme\iSeconds Icons Export.ibp
   - Mandar rodar (Start batch convert...)
2- Rodar a bat que copia os arquivos para o diret�rio correto. Fica em Glyphs\theme\setUp.bat
3- No Xamarin Studio usar a configura��o Release
4- Ir em Project\Publish Android Application.
5- Vair solicitar a keystore:
   - Marcar Use existing keystore
   - Abrir o arquivo no raiz do projeto: yester.keystore
   - Password: yestermanobrodi
   - Alias: yester
   - Key password: yestermanobrodi
6- Selecionar o destino:
   - Diret�rio "iSeconds.Droid\bin\Release"
   - Colocar o nome do arquivo: Yester.Droid-Signed
   - Antes de criar, ir no diret�rio de alvo e apagar o arquivo Yester.Droid-Signed.apk se ele existir l�. Isso � porque n�o confio no update do Xamarin Studio
7- Rodar a bat do zipalign. No diret�rio raiz e rodar o run_zipalign.bat. Para ter certeza que rodou, abrir o cmd e rodar por l�.
8- Se tudo deu certo, vai ter o Yester.Droid.apk no raiz do projeto.


Build Yester Lite
1- Gerar as imagens:
   - Abrir o leitor dos scripts. Pasta Glyphs\InkscapeBatch\InkscapeBatchGUI.Exe
   - Abrir o script em Glyphs\theme\iSeconds Icons Export_lite.ibp
   - Mandar rodar (Start batch convert...)
2- Rodar a bat que copia os arquivos para o diret�rio correto. Fica em Glyphs\theme\setUp.bat
3- Rodar o script para alterar os namespaces. Ir em Build\build_for_yester_lite.bat
4- No Xamarin Studio usar a configura��o Release
5- Ir em Project\Publish Android Application.
6- Vair solicitar a keystore:
   - Marcar Use existing keystore
   - Abrir o arquivo no raiz do projeto: yester.lite.keystore
   - Password: yestermanobrodi
   - Alias: yester.lite
   - Key password: yestermanobrodi
7- Selecionar o destino:
   - Diret�rio "iSeconds.Droid\bin\Release Lite"
   - Colocar o nome do arquivo: Yester.Droid.Lite-Signed
   - Antes de criar, ir no diret�rio de alvo e apagar o arquivo Yester.Droid.Lite-Signed.apk se ele existir l�. Isso � porque n�o confio no update do Xamarin Studio
8- Rodar a bat do zipalign. No diret�rio raiz e rodar o run_zipalign_lite.bat. Para ter certeza que rodou, abrir o cmd e rodar por l�.
9- Se tudo deu certo, vai ter o Yester.Droid.apk no raiz do projeto.


------------------
Pra criar o keystore do Yester, foi usado o JDK6_45