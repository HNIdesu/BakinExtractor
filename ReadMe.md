# BakinExtractor

## Introduction
BakinExtractor is a tool designed to unpack and repack resource files (`data.rbpack`) created by RPG Developer Bakin.

## Usage

### Packing
To pack a directory into a `data.rbpack` file, use the following command:
```
bakin-extractor pack <unpack_directory> <packed_file_path>
```
- `<unpack_directory>`: The directory containing the files you want to pack.
- `<packed_file_path>`: The path where the packed file will be saved.

### Unpacking
To unpack a `data.rbpack` file, use the following command:
```
bakin-extractor unpack <rbpack_path> <output_directory>
```
- `<rbpack_path>`: The path to the `data.rbpack` file you want to unpack.
- `<output_directory>`: The directory where the unpacked files will be saved.

## Examples

### Packing Example
```
bakin-extractor pack ./unpacked_data ./packed_data.rbpack
```

### Unpacking Example
```
bakin-extractor unpack ./data.rbpack ./output_data
```

## License
This project is licensed under the MIT License. See the [LICENSE](./LICENSE) file for details.