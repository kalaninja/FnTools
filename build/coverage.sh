#!/bin/bash

set -e

declare -r ROOT=$(realpath $(dirname $0)/..)
declare -r TEST=$ROOT/tests/FnTools.Tests
declare -r DOTCOVER_VERSION=2019.3.4

nuget.exe install -Verbosity quiet -OutputDirectory $ROOT/packages -Version $DOTCOVER_VERSION JetBrains.dotCover.CommandLineTools

declare -r DOTCOVER=$ROOT/packages/JetBrains.dotCover.CommandLineTools.$DOTCOVER_VERSION/tools/dotCover.exe

rm -rf $ROOT/coverage
mkdir $ROOT/coverage

(cd $TEST; 
 $DOTCOVER dotnet --output=$ROOT/coverage/coverage.xml --reportType=DetailedXML -- test)

choco install codecov
codecov -f coverage/coverage.xml