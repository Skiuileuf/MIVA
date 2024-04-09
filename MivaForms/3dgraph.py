import numpy as np
import matplotlib.pyplot as plt
from mpl_toolkits.mplot3d import Axes3D
import csv

# Read data from CSV file and skip the header
data = []
with open('efficientProduction.csv', 'r') as file:
    reader = csv.reader(file)
    next(reader)  # Skip the header
    for row in reader:
        data.append([float(x) for x in row])

# Filter data with score greater than -1
filtered_data = [point for point in data if point[0] != -1]

# Extract x, y, z coordinates and scores
x = [int(point[1]) for point in filtered_data]  # Pech
y = [int(point[2]) for point in filtered_data]  # Prom
z = [int(point[3]) for point in filtered_data]  # Stan
score = [point[0] for point in filtered_data]

# Create a 3D array representing voxels
voxels = np.zeros((max(x)+1, max(y)+1, max(z)+1), dtype=int)
for i in range(len(x)):
    voxels[x[i], y[i], z[i]] = score[i]

# Plot the voxels
fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')

# Define a colormap
cmap = plt.cm.viridis

# Normalize the scores to be used as color values
norm = plt.Normalize(min(score), max(score))

# Create colored voxels
colors = cmap(norm(voxels))
ax.voxels(voxels, facecolors=colors, edgecolors='k')  # Adjust colors as needed

# Add color bar
cbar = plt.colorbar(plt.cm.ScalarMappable(norm=norm, cmap=cmap), ax=ax)
cbar.set_label('Score')

ax.set_xlabel('Pech')
ax.set_ylabel('Prom')
ax.set_zlabel('Stan')
ax.set_title('Variants with Score > -1')
plt.show()
