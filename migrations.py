#!/usr/bin/python3

import os
import sys

PROJECT_DIR = 'src/api/Diagraph.Api'
SCRIPT_DIR = os.getcwd()

def run_command(command, *args):
    os.chdir(PROJECT_DIR)
    result = command(*args)
    os.chdir(SCRIPT_DIR)

    return result


def run_with_project(command):
    return os.system(command + ' --project ../Diagraph.Infrastructure')


def add_migration(name):
    return run_with_project('dotnet ef migrations add ' + name)


def apply_migrations():
    return run_with_project('dotnet ef database update --context DiagraphDbContext')


def list_migrations():
    return run_with_project('dotnet ef migrations list')


def remove_last_migration():
    return run_with_project('dotnet ef migrations remove')


if __name__ == '__main__':
    command_name = sys.argv[1]

    result = 0

    match command_name:
        case 'add':    result = run_command(add_migration, sys.argv[2])
        case 'apply':  result = run_command(apply_migrations)
        case 'list':   result = run_command(list_migrations)
        case 'remove': result = run_command(remove_last_migration)
        case _:        sys.exit('Could not recognize command \'' + command_name + '\'')

    sys.exit(result)

