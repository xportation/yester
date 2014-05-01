import os, fnmatch

def findReplace(directory, find, replace, filePattern):
    for path, dirs, files in os.walk(os.path.abspath(directory)):
        for filename in fnmatch.filter(files, filePattern):
            filepath = os.path.join(path, filename)
            with open(filepath, "r") as f:
                s = f.read()
            s2 = s.replace(find, replace)
            with open(filepath, "w") as f:
                f.write(s2)  