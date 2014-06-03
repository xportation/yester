Tags revisions:
Tag 1.00.01 - 225/224
Tag 1.01.01 - 252/251

--------------------
Passos para gerar uma versão release.
Antes de gerar a versão conferir se precisa aumentar a versão do app nas suas configuraçõs.
NÃO fazer commit da versão lite. Ou seja, se vc rodar o script para versão lite, não fazer o commit. Isso não é regra, é uma convençã. Afinal vc pode desenvolver com a Lite só usando as configuraçõs de build no Xamarin Studio.


Pré-requisitos:
1. Inkscape 0.48 instalado
2. Python instalado e diretório no Path. (uso o Python 3.3)

Build Yester Full
1. Gerar as imagens:
   * Abrir o leitor dos scripts. Pasta Glyphs\InkscapeBatch\InkscapeBatchGUI.Exe
   * Abrir o script em Glyphs\theme\iSeconds Icons Export.ibp
   * Mandar rodar (Start batch convert...)
2. Rodar a bat que copia os arquivos para o diretório correto. Fica em Glyphs\theme\setUp.bat
3. No Xamarin Studio usar a configuração Release
4. Ir em Project\Publish Android Application.
5. Vair solicitar a keystore:
   * Marcar Use existing keystore
   * Abrir o arquivo no raiz do projeto: yester.keystore
   * Password: yestermanobrodi
   * Alias: yester
   * Key password: yestermanobrodi
6. Selecionar o destino:
   * Diretório "iSeconds.Droid\bin\Release"
   * Colocar o nome do arquivo: Yester.Droid-Signed
   * Antes de criar, ir no diretório de alvo e apagar o arquivo Yester.Droid-Signed.apk se ele existir lá. Isso é porque não confio no update do Xamarin Studio
7. Rodar a bat do zipalign. No diretório raiz e rodar o run_zipalign.bat. Para ter certeza que rodou, abrir o cmd e rodar por lá.
8. Se tudo deu certo, vai ter o Yester.Droid.apk no raiz do projeto.


Build Yester Lite
1. Gerar as imagens:
   * Abrir o leitor dos scripts. Pasta Glyphs\InkscapeBatch\InkscapeBatchGUI.Exe
   * Abrir o script em Glyphs\theme\iSeconds Icons Export_lite.ibp
   * Mandar rodar (Start batch convert...)
2. Rodar a bat que copia os arquivos para o diretório correto. Fica em Glyphs\theme\setUp.bat
3. Rodar o script para alterar os namespaces. Ir em Build\build_for_yester_lite.bat
4. No Xamarin Studio usar a configuração Release
5. Ir em Project\Publish Android Application.
6. Vair solicitar a keystore:
   * Marcar Use existing keystore
   * Abrir o arquivo no raiz do projeto: yester.lite.keystore
   * Password: yestermanobrodi
   * Alias: yester.lite
   * Key password: yestermanobrodi
7. Selecionar o destino:
   * Diretório "iSeconds.Droid\bin\Release Lite"
   * Colocar o nome do arquivo: Yester.Droid.Lite-Signed
   * Antes de criar, ir no diretório de alvo e apagar o arquivo Yester.Droid.Lite-Signed.apk se ele existir lá. Isso é porque não confio no update do Xamarin Studio
8. Rodar a bat do zipalign. No diretório raiz e rodar o run_zipalign_lite.bat. Para ter certeza que rodou, abrir o cmd e rodar por lá.
9. Se tudo deu certo, vai ter o Yester.Droid.apk no raiz do projeto.


------------------
Pra criar o keystore do Yester, foi usado o JDK6_45
