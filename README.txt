Pra criar o keystore do Yester, foi usado o JDK6_45
o alias � yester
senha yestermanobrodi
para os dois que pede

ao gerar, usar o nome Yester.Droid-Signed.apk
pois o run_zipalign.bat vai renomear para Yester.Droid.apk

depois precisa rodar o zipalign, notar que a bat tem diretorios hardcode.
para ele, eu gerei o Yester.Droid-Signed.apk
ele gera o Yester.Droid.apk

------------
Para a vers�o lite, � preciso gerar os �cones e rodar o script para alterar
o namespace do app. O script fica na pasta Build do raiz
j� os icones em Glyph\theme
Usar o yester.lite.keystore e compilar na configura��o Release Lite