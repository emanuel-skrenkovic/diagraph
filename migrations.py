#!/usr/bin/python3

import os
import sys

def add_migration(name):
    before = os.getcwd()
    os.chdir('src/Diagraph.Api')

    result = os.system('dotnet ef migrations add ' + name + ' --project ../Diagraph.Infrastructure')

    os.chdir(before)

    return result


def apply_migrations():
    before = os.getcwd()
    os.chdir('src/Diagraph.Api')

    result = os.system('dotnet ef database update --context DiagraphDbContext --project ../Diagraph.Infrastructure')

    os.chdir(before)

    return result


if __name__ == '__main__':
    command_name = sys.argv[1]

    result = 0

    if command_name == 'add':
        result = add_migration(sys.argv[2])
    elif command_name == 'apply':
        result = apply_migrations()

    sys.exit(result)

