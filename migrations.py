#!/usr/bin/python3

import os
import sys


PROJECT_DIR = 'src/api/Diagraph.Api'
SCRIPT_DIR = os.getcwd()


migrations = {
    'EventsDbContext':      '../Modules/Events/Modules.Events',
    'IdentityDbContext':    '../Modules/Identity/Modules.Identity',
    'GlucoseDataDbContext': '../Modules/GlucoseData/Modules.GlucoseData'
}


def run_command(command, context, *args):
    os.chdir(PROJECT_DIR)
    result = command(context, *args)
    os.chdir(SCRIPT_DIR)

    return result


def run_with_project(context, command):
    full_command = command + ' --context ' + context + ' --project ' + migrations[context] + ' --startup-project Diagraph.Api.csproj';
    print('Running command:\n' + full_command)
    return os.system(full_command)


def add_migration(context, name):
    return run_with_project(context, 'dotnet ef migrations add ' + name)


def apply_migrations(context=None):
    # TODO: think about doing the same for all commands - running for all contexts if none is passed in.
    if context is None:
        for key in migrations:
            run_with_project(key, 'dotnet ef database update')
    else:
        return run_with_project(context, 'dotnet ef database update')


def list_migrations(context):
    return run_with_project(context, 'dotnet ef migrations list')


def remove_last_migration(context):
    return run_with_project(context, 'dotnet ef migrations remove')


if __name__ == '__main__':
    result = 0


    if len(sys.argv) > 2:
        context = sys.argv[1]
        command_name = sys.argv[2]
    else:
        context = None
        command_name = sys.argv[1]

    match command_name:
        case 'add':    result = run_command(add_migration, context, sys.argv[3])
        case 'apply':  result = run_command(apply_migrations, context)
        case 'list':   result = run_command(list_migrations, context)
        case 'remove': result = run_command(remove_last_migration, context)
        case _:        sys.exit('Could not recognize command \'' + command_name + '\'')

    sys.exit(result)

