import os
import subprocess
import time

import os
 
dirpath = os.getcwd()
print("current directory is : " + dirpath)

magickInstall = "\"c:\\Program Files\\ImageMagick-7.0.8-Q16\\magick.exe\""

imagesProcessed = 0
start = time.time()

def convertIcons(originDirectory, destDirectory, color, radius, strength):
	global imagesProcessed
	for filename in os.listdir(originDirectory):
		magickcommand = magickInstall + " convert " + originDirectory + filename + " ( +clone  -background " + color + " -shadow " + strength + "x" + radius + "+0+0 ) +swap -background none -layers merge +repage " + destDirectory + filename
		print(filename)
		os.system(magickcommand)
		imagesProcessed = imagesProcessed+1

def convertIconsParallel(originDirectory, destDirectory, color, radius, strength, overwriteExistingOnes):
	global imagesProcessed
	processes = set()
	max_processes = 8

	for filename in os.listdir(originDirectory):
		if (os.path.isfile(destDirectory + filename)):
			continue
		magickcommand = magickInstall + " convert " + originDirectory + filename + " ( +clone  -background " + color + " -shadow " + strength + "x" + radius + "+0+0 ) +swap -background none -layers merge +repage " + destDirectory + filename
		print(filename)
		processes.add(subprocess.Popen(magickcommand))
		imagesProcessed = imagesProcessed+1
		while len(processes) >= max_processes:
			time.sleep(.1)
			processes.difference_update([
				p for p in processes if p.poll() is not None])





# If you add a # in the beginning, the line is commented

# color, radius, strength (0-100)
convertIconsParallel("PhotoStudioIconsBig/", "PhotoStudioIconsProcessed/", "rgb(255,255,255)", "25", "15", False)
convertIconsParallel("PhotoStudioIconsMedium/", "PhotoStudioIconsProcessed/", "rgb(255,255,255)", "15", "15", False)

#convertIconsParallel("FurnitureIconsBig/", "../../Assets/Resources/FurnitureIcons/", "rgb(255,255,255)", "30", "15", False)
#convertIconsParallel("FurnitureIcons/", "../../Assets/Resources/FurnitureIcons/", "rgb(255,255,255)", "15", "15", False)
#convertIconsParallel("EquipmentIconsBig/", "../../Assets/Resources/EquipmentIcons/", "rgb(255,255,255)", "20", "15", False)
#convertIconsParallel("EquipmentIcons/", "../../Assets/Resources/EquipmentIcons/", "rgb(255,255,255)", "15", "15", False)

#convertIconsParallel("FoundersArmor/Big/", "../../Assets/AssetBundles/FoundersArmor/", "rgb(255,255,255)", "20", "15", False)
#convertIconsParallel("FoundersArmor/Small/", "../../Assets/AssetBundles/FoundersArmor/", "rgb(255,255,255)", "15", "15", False)

#convertIconsParallel("FoundersFurniture/Big/", "../../Assets/AssetBundles/FoundersFurniture/Icons/", "rgb(255,255,255)", "30", "15", False)
#convertIconsParallel("FoundersFurniture/Small/", "../../Assets/AssetBundles/FoundersFurniture/Icons/", "rgb(255,255,255)", "15", "15", False)


end = time.time()
seconds = end - start

print(str(imagesProcessed) + " Images processed in " + str(seconds) + " seconds")
if imagesProcessed > 0:
	print("Millisecs per image: " + str(seconds / imagesProcessed*1000))
input("Press Enter to continue...")