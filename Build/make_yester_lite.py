import os
from find_replace import findReplace

full_path = os.path.realpath(__file__)
my_path = os.path.dirname(full_path) + "/../iSeconds.Droid/"
print(my_path)

findReplace(my_path, "iSeconds.Droid", "iSeconds.Droid.Lite", "*.xml")
findReplace(my_path, "iSeconds.Droid", "iSeconds.Droid.Lite", "*.axml")
findReplace(my_path, "iSeconds.Droid", "iSeconds.Droid.Lite", "*.cs")
