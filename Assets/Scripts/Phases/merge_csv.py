import os
import pandas as pd

def merge_csv_files(folder_path, output_file):
    # List to store the data from each CSV file
    data_frames = []

    # Iterate over all files in the given folder
    for file_name in os.listdir(folder_path):
        if file_name.endswith('.csv'):
            file_path = os.path.join(folder_path, file_name)
            # Read the CSV file into a DataFrame
            df = pd.read_csv(file_path, header=None)
            data_frames.append(df)
    
    # Concatenate all DataFrames along columns
    merged_df = pd.concat(data_frames, axis=1)

    # Save the merged DataFrame to a new CSV file
    merged_df.to_csv(output_file, index=False, header=False)

# Example usage
folder_path = '/Users/alfhusem/Downloads/New Folder With Items 2/HeartRates'  # Replace with the path to your folder
output_file = 'merged_output.csv'    # Replace with your desired output file name
merge_csv_files(folder_path, output_file)
