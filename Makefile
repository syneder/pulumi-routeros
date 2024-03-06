WORKING_DIR	:= $(shell pwd)

provider::
	@echo > /dev/null \
		&& cd $(WORKING_DIR)/provider/Pulumi.RouterOS \
		&& dotnet build --output $(WORKING_DIR)/bin --configuration Release

dotnet_sdk:: DOTNET_VERSION := $(shell pulumictl get version --language dotnet)
dotnet_sdk::
	@echo > /dev/null \
		&& rm -rf sdk/dotnet \
		&& pulumi package gen-sdk $(WORKING_DIR)/bin/Pulumi.RouterOS --language dotnet \
		&& cd $(WORKING_DIR)/sdk/dotnet \
		&& echo "${DOTNET_VERSION}" > version.txt \
		&& dotnet build /p:Version=${DOTNET_VERSION}
