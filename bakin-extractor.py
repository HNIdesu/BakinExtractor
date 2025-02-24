from pack import pack
from unpack import unpack
from argparse import ArgumentParser

parser = ArgumentParser()
subparsers = parser.add_subparsers(dest="verb")

parser_pack = subparsers.add_parser("pack")
parser_pack.add_argument("unpack_directory",type=str)
parser_pack.add_argument("packed_file_path",type=str)

parser_unpack = subparsers.add_parser("unpack")
parser_unpack.add_argument("rbpack_path",type=str)
parser_unpack.add_argument("output_directory",type=str)

args = parser.parse_args()
if args.verb == "pack":
    pack(args.unpack_directory,args.packed_file_path)
elif(args.verb == "unpack"):
    unpack(args.rbpack_path,args.output_directory)
else:
    parser.print_help()