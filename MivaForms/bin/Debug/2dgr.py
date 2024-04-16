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
x = [point[1] for point in filtered_data]  # Pech
y = [point[2] for point in filtered_data]  # Prom
z = [point[3] for point in filtered_data]  # Stan
score = [point[0] for point in filtered_data]

# Plot 3D scatter plot
fig = plt.figure()
ax = fig.add_subplot(111, projection='3d')
ax.scatter(x, y, z, c=score, cmap='viridis')
ax.set_xlabel('Pech')
ax.set_ylabel('Prom')
ax.set_zlabel('Stan')
ax.set_title('Variants with Score > -1')
plt.show()