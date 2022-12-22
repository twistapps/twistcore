# [0.17.0](https://github.com/twistapps/twistcore/compare/0.16.2...0.17.0) (2022-12-22)


### Bug Fixes

* Prettier progress display ([58e7055](https://github.com/twistapps/twistcore/commit/58e70554e2a6305f2980e22609a9828b6835db1e))


### Features

* SyncFolder ignore mask ([7761a71](https://github.com/twistapps/twistcore/commit/7761a71912961bab4fe86ffc0f1223b37912b4f0))

## [0.16.2](https://github.com/twistapps/twistcore/compare/0.16.1...0.16.2) (2022-12-22)


### Bug Fixes

* Overwrite errors ([190e9c4](https://github.com/twistapps/twistcore/commit/190e9c47fc512ae94b8aa7fcf0d9b3b0d01483e0))

## [0.16.1](https://github.com/twistapps/twistcore/compare/0.16.0...0.16.1) (2022-12-22)


### Bug Fixes

* Output folder ([e368fc9](https://github.com/twistapps/twistcore/commit/e368fc9b706bccb4c684fe7675c687da4a5b0944))

# [0.16.0](https://github.com/twistapps/twistcore/compare/0.15.1...0.16.0) (2022-12-22)


### Bug Fixes

* Avoid code dupl. by synchronous task exec ([39cda8e](https://github.com/twistapps/twistcore/commit/39cda8e30b354e449a8b879725085cff5060d99a))
* Invoke OnComplete actions if window is inactive ([54d6ea1](https://github.com/twistapps/twistcore/commit/54d6ea1915289cb5901c1827d46f7dcdc5add7a0))
* Package name, Folder paths ([f61d9f7](https://github.com/twistapps/twistcore/commit/f61d9f786a4ec34b2bd21fa30565737e858dc58a))
* Remove redundant code, fix typo ([419d149](https://github.com/twistapps/twistcore/commit/419d149140335704edf12a158f2f3251c912a0af))
* Reset task sleeping time after sleep ([d3bbfab](https://github.com/twistapps/twistcore/commit/d3bbfab3c4c36cb39370539a8e34ad856ad7cda4))


### Features

* Collect ScriptTemplates from supported pkgs ([971bf7c](https://github.com/twistapps/twistcore/commit/971bf7c3345d545a3f2cb4b90430a0e76c8e2496))
* Finish task synchronously ([5b19715](https://github.com/twistapps/twistcore/commit/5b19715f82c734fb9652cd4fa22a333f6d211fe8))
* FolderSync class ([01487c7](https://github.com/twistapps/twistcore/commit/01487c757e315dd65fbb7ab31ff9f493315284e8))
* Record latest log pushed by current task ([3eb57f8](https://github.com/twistapps/twistcore/commit/3eb57f8a6e37f6bcfba7830e53ef6b6630a7780e))

## [0.15.1](https://github.com/twistapps/twistcore/compare/0.15.0...0.15.1) (2022-12-21)


### Bug Fixes

* 2-Level nested sections support ([9fe127a](https://github.com/twistapps/twistcore/commit/9fe127af4828c2db7343861cdc0d5d603e7b9059))
* Add Space() to interface ([50b52f1](https://github.com/twistapps/twistcore/commit/50b52f174ea27d85c315f9a1e73974c97b425861))
* Embedding core lib is unnecessary ([e94ae4c](https://github.com/twistapps/twistcore/commit/e94ae4cdb74bfeb2fe3240a3144e744a6cc76960))
* Reload packages if stuck ([d55ce46](https://github.com/twistapps/twistcore/commit/d55ce469e95cf1e77efe068d61102f5289502e05))
* Remove debug "show ProgressWindow" menu item ([07d079c](https://github.com/twistapps/twistcore/commit/07d079c689af1e5239a3ecc74a5a6a7831250593))

# [0.15.0](https://github.com/twistapps/twistcore/compare/0.14.0...0.15.0) (2022-12-21)


### Bug Fixes

* Don't copy git folder on unpack ([03d893b](https://github.com/twistapps/twistcore/commit/03d893b96147fd660c5427ba5c0a5a050188e936))
* Path fields access ([f316aaa](https://github.com/twistapps/twistcore/commit/f316aaa38c72a6e38f5cbe6de5ba266191bb850d))
* Purge pkg cache on manifest source change ([3786b75](https://github.com/twistapps/twistcore/commit/3786b75d2773d578362bb9c7555bbaea569875b8))
* Refactor - cleanup ([8352b0b](https://github.com/twistapps/twistcore/commit/8352b0bc5ab7075fca95db6cfcaf958dc98aa1c2))
* Update manifest ([1e7bbe8](https://github.com/twistapps/twistcore/commit/1e7bbe8bf8042af8833725d9674565da6ac4e488))


### Features

* Add dependencies not listed in manifest ([3f0171b](https://github.com/twistapps/twistcore/commit/3f0171bc22f26e402c0c778383739317ba6197b3))
* Cache full package collection ([5fb8454](https://github.com/twistapps/twistcore/commit/5fb845458ea9408cb7824b2ac30babc50ba0d6f0))
* Embed request ([74ae071](https://github.com/twistapps/twistcore/commit/74ae071a709ce72f951cb948a38398dfcd442a97))
* Get package from manifest ([88f726b](https://github.com/twistapps/twistcore/commit/88f726b6d39618c7199a50c6d6d8504456b53620))

# [0.14.0](https://github.com/twistapps/twistcore/compare/0.13.1...0.14.0) (2022-12-19)


### Bug Fixes

* DependencyManager window overhaul ([a251a79](https://github.com/twistapps/twistcore/commit/a251a791fcd89552a8079e128ea2accc7d24c59d))
* Load packages by manifest instead of name mask ([358f298](https://github.com/twistapps/twistcore/commit/358f2985cfe3c338e80cae40014312d6787d604e))
* Refactor ([47d65d6](https://github.com/twistapps/twistcore/commit/47d65d611c0e9ea771219ed5f282586257a64516))
* Update package-manifest.json ([b7840b2](https://github.com/twistapps/twistcore/commit/b7840b26449a5dbbc0ed483e91fcb0f3d0c3f3d8))


### Features

* Check if requested package is installed ([5d225bb](https://github.com/twistapps/twistcore/commit/5d225bb083ed28f235d0388f1920d65d3a03e499))
* DependencyList window ([a7db30a](https://github.com/twistapps/twistcore/commit/a7db30a4a7836966a243508c3302a84944c3f6ff))
* Fetch update info in PackageData ([8cc5271](https://github.com/twistapps/twistcore/commit/8cc5271cfed2e0830e88724f92d6be26ae22f7ec))
* Install Package ([4d0c13c](https://github.com/twistapps/twistcore/commit/4d0c13c29fe2d70a715a2864b6489ab10f3a0924))
* IsGithubPackage ([3fdd366](https://github.com/twistapps/twistcore/commit/3fdd366ef141f4dc934575760b94255f0dfac731))
* List packages in project ([3a06210](https://github.com/twistapps/twistcore/commit/3a062107724db5022cbe3c35e4e7c3caa4c99a3c))
* Manifest: UpdateDeps. RemovePkg, Save ([6ddf7bf](https://github.com/twistapps/twistcore/commit/6ddf7bf744fab64f5364e3f9df2a27126e164dfa))
* resolve [#4](https://github.com/twistapps/twistcore/issues/4) ([975d4bc](https://github.com/twistapps/twistcore/commit/975d4bc7ff435950594a1a842ed414ceedf1c2d0))

## [0.13.1](https://github.com/twistapps/twistcore/compare/0.13.0...0.13.1) (2022-11-07)


### Bug Fixes

* Add DependencyManager settings to unpackignore ([9e15014](https://github.com/twistapps/twistcore/commit/9e1501437fac93543fb83bcff61977d50901de28))
* Convert PackageData to PackageInfo ([e32579a](https://github.com/twistapps/twistcore/commit/e32579aa378dd536f8f77c775e2f8f7a7bab37a8))
* Сomprehensive cleanup ([a3b7162](https://github.com/twistapps/twistcore/commit/a3b71629c37741873c35f9f58cc2b883280020a2))

# [0.13.0](https://github.com/twistapps/twistcore/compare/0.12.0...0.13.0) (2022-11-07)


### Bug Fixes

* Match UI Framework interface to latest update ([130b3d5](https://github.com/twistapps/twistcore/commit/130b3d505733344c6cf3572e3db0766c7bca225d))
* NullReferenceException probs when UPM auth is inactive ([01d382d](https://github.com/twistapps/twistcore/commit/01d382dc73364654cc9abc9adcbe58b40a996c6c))
* Small UI Improvements ([2b309e0](https://github.com/twistapps/twistcore/commit/2b309e0c34da706d7638b2d1cfa94adc0f3893f8))


### Features

* Manifest editor improvements ([9cf2007](https://github.com/twistapps/twistcore/commit/9cf2007cc32de7d6acf505992297f50a45f99aba))
* Support multiple !except entries, divided by & symbol ([a75af71](https://github.com/twistapps/twistcore/commit/a75af718c051ea6f502f01a2d6ced0883262f46d))

# [0.12.0](https://github.com/twistapps/twistcore/compare/0.11.0...0.12.0) (2022-11-07)


### Features

* Add Modula to package-manifest ([6499408](https://github.com/twistapps/twistcore/commit/649940848757cb5b8de1fa19c985931854db9013))
* PackageInfo to PackageData conversion ([535f603](https://github.com/twistapps/twistcore/commit/535f6032c590a0d23e1740ced639a7a64e04efbc))
* PackagesInProject list widget ([5505d2e](https://github.com/twistapps/twistcore/commit/5505d2e152fec0570d37de5669aec1e8e108cbea))

# [0.11.0](https://github.com/twistapps/twistcore/compare/0.10.4...0.11.0) (2022-11-06)


### Bug Fixes

* Idling forever after completing a task ([3fbc80e](https://github.com/twistapps/twistcore/commit/3fbc80e740cbd8f71adb2843198bfca0f67077cf))
* Matching [#2](https://github.com/twistapps/twistcore/issues/2) ([0f6c4cc](https://github.com/twistapps/twistcore/commit/0f6c4cc5b8c6331530bd6e652d45d2644b42360f))
* Possibe skip of wait on tasks complete ([fabf494](https://github.com/twistapps/twistcore/commit/fabf4947a7394160fb2369329b1be1cfed0234b9))
* Remove Duplicate const ([1c5dbf3](https://github.com/twistapps/twistcore/commit/1c5dbf30f076b0b8e09b71a93e73ac85176cd2e0))


### Features

* Create manifest ([d030921](https://github.com/twistapps/twistcore/commit/d0309217ae3d73a8b7d69376c744bfce607dfe91))
* Dependency Manager ([345f67a](https://github.com/twistapps/twistcore/commit/345f67ac222e4ce9617e86a62f951d0bb139570f))
* New Definitions ([48a781f](https://github.com/twistapps/twistcore/commit/48a781f6a9ab5afea643b95295194a4531f4405c))
* UI Framework Overhaul (closes [#2](https://github.com/twistapps/twistcore/issues/2)) ([fbed659](https://github.com/twistapps/twistcore/commit/fbed659dd6d209449d9696c845b3f1a93bbc489d))
* WebRequestUtility ([75b8632](https://github.com/twistapps/twistcore/commit/75b863242b4e5e1b43a5de83a3539c6419a5a306))

## [0.10.4](https://github.com/twistapps/twistcore/compare/0.10.3...0.10.4) (2022-11-06)


### Bug Fixes

* Add Scroll view for UI overflow ([5b79213](https://github.com/twistapps/twistcore/commit/5b79213a8405f83096a19709abd84fab86c2e829))
* Refactor UI layout ([408f835](https://github.com/twistapps/twistcore/commit/408f83590ff6ffe59fc06bfd2daf721dc867b6ee))

## [0.10.3](https://github.com/twistapps/twistcore/compare/0.10.2...0.10.3) (2022-11-06)


### Bug Fixes

* Refactor & Cleanup ([0eae8dd](https://github.com/twistapps/twistcore/commit/0eae8ddd03f0a7e047a1a048f6b47f9f676d0a9d))

## [0.10.2](https://github.com/twistapps/twistcore/compare/0.10.1...0.10.2) (2022-11-06)


### Bug Fixes

* Automated Cleanup ([36caf5f](https://github.com/twistapps/twistcore/commit/36caf5fd21c2148642aba058ed5c707900df10ae))
* Comment excess logs ([b513b17](https://github.com/twistapps/twistcore/commit/b513b179628f6aba22a1fbf4f751f61fef7efeea))
* Update .gitignore ([115efea](https://github.com/twistapps/twistcore/commit/115efeaf1ac8a4abc3689d5399ae6745faee9e2d))

## [0.10.1](https://github.com/twistapps/twistcore/compare/0.10.0...0.10.1) (2022-11-06)


### Bug Fixes

* add missing metafile ([06cef21](https://github.com/twistapps/twistcore/commit/06cef210ec0e77f7a406559eef148ae2ce1b582f))
* asmdef references. Add Editor Coroutines ([0236167](https://github.com/twistapps/twistcore/commit/0236167d26afa01652e63c15bb0e5125d31e5c25))
* EditorData ref and Dev mode ([e748948](https://github.com/twistapps/twistcore/commit/e7489482d8767858e133b9b31375544079327152))
* Script reference ([fb35cc7](https://github.com/twistapps/twistcore/commit/fb35cc73ce2d8d7ff634560df13b865ed0d6e50c))

# [0.10.0](https://github.com/twistapps/twistcore/compare/0.9.0...0.10.0) (2022-11-06)


### Features

* Core Unpack task ([ab24e79](https://github.com/twistapps/twistcore/commit/ab24e791f9e72994af9fe3032a1ee6ff8a22add0))

# [0.9.0](https://github.com/twistapps/twistcore/compare/0.8.0...0.9.0) (2022-11-06)


### Features

* TaskManager and ProgressWindow ([bbf8c73](https://github.com/twistapps/twistcore/commit/bbf8c732915119c4f4dca0eb72c81d9a0ad404a5))

# [0.8.0](https://github.com/twistapps/twistcore/compare/0.7.1...0.8.0) (2022-11-06)


### Bug Fixes

* Better naming ([c376b1b](https://github.com/twistapps/twistcore/commit/c376b1be32c9d1212fdce8d12112a4579310dd2d))
* Moved to PersistentEditorData ([f2eedd8](https://github.com/twistapps/twistcore/commit/f2eedd8b5acf7001684dff0b99a26e71cb088ac8))
* TrimRoot() access level ([8fe01d8](https://github.com/twistapps/twistcore/commit/8fe01d813f13da13b511b33a5010088760b2eaba))


### Features

* Create GetPackageName() in SettingsAsset ([d2c20e6](https://github.com/twistapps/twistcore/commit/d2c20e64d7f5bf2f031c9f6fedd45e966104fe52))
* Improve package unpacking, restore asmdef function ([c7f41a3](https://github.com/twistapps/twistcore/commit/c7f41a335ae003852530a231c03bac67b2b28624))
* PersistentEditorData singleton ([5cb8bb0](https://github.com/twistapps/twistcore/commit/5cb8bb055ddaacb0f99cb71696e7f20d3770f413))
* UI Components ([7940b9c](https://github.com/twistapps/twistcore/commit/7940b9cc6b974aae35a86c8c9a7020cc5dc24403))

## [0.7.1](https://github.com/twistapps/twistcore/compare/0.7.0...0.7.1) (2022-10-22)


### Bug Fixes

* Add missing meta files ([3baba36](https://github.com/twistapps/twistcore/commit/3baba36c9348a6fdc760f13c0f0404d55b5c2db0))

# [0.7.0](https://github.com/twistapps/twistcore/compare/0.6.0...0.7.0) (2022-10-22)


### Features

* Retrieve git version on startup ([a7d25a7](https://github.com/twistapps/twistcore/commit/a7d25a7b09cdea1e5645a13df9110255290e1440))
* settings window ([c383af2](https://github.com/twistapps/twistcore/commit/c383af29d6b74ab16eee9149bd2813cc7e38283c))
* Unpack core to any in-dev package ([ff00545](https://github.com/twistapps/twistcore/commit/ff00545521dc822410bfc4b25b444f8dbc114985))

# [0.6.0](https://github.com/twistapps/twistcore/compare/0.5.0...0.6.0) (2022-10-22)


### Bug Fixes

* ShowWindow now has out Window arg ([5d0463f](https://github.com/twistapps/twistcore/commit/5d0463fdd8f65ac3411993a5cbdd18ee1adf4a09))


### Features

* Create element: StatusLabel ([1b6fc6c](https://github.com/twistapps/twistcore/commit/1b6fc6c43128cf2a9abf36345d2c332e3dd2a1d1))
* Customize buttons' width ([15e64bf](https://github.com/twistapps/twistcore/commit/15e64bf2ff0dcbcfe5384b402c2050ff6c628045))
* Disable/add toggle to sections ([575c25f](https://github.com/twistapps/twistcore/commit/575c25f1000d6cffe979fb01b3f672614bac828a))
* QoL: Singular horizontal button section ([6ed1a02](https://github.com/twistapps/twistcore/commit/6ed1a027587411c849dfc542f4e30aba80ffe0ed))

# [0.5.0](https://github.com/twistapps/twistcore/compare/0.4.0...0.5.0) (2022-10-22)


### Bug Fixes

* Create asmdef ([f146e00](https://github.com/twistapps/twistcore/commit/f146e00ef72fb568504163eb9429e15db4c97608))
* Namespace ([db60471](https://github.com/twistapps/twistcore/commit/db60471ed95afea0f3d2e821baec971afc95b4a9))


### Features

* Add third party lib (Json.NET) ([1b21691](https://github.com/twistapps/twistcore/commit/1b21691a35faabd78ea98d59c8604807ce0a4bac))
* Fetch and deserialize package.json from url ([d92fd20](https://github.com/twistapps/twistcore/commit/d92fd20830848b9968037625ff2de393cc5854a8))
* FW Extensions for PackageInfo/PackageData ([ac681fb](https://github.com/twistapps/twistcore/commit/ac681fba6b1b09a40aa15a6dfdd3c1f45cc678d3))
* Inquire package source from lockfile ([2907c14](https://github.com/twistapps/twistcore/commit/2907c148201b4195eeb56ac883350956a00205e9))

# [0.4.0](https://github.com/twistapps/twistcore/compare/0.3.0...0.4.0) (2022-10-22)


### Features

* Add package template for creation tool ([aca2a51](https://github.com/twistapps/twistcore/commit/aca2a5186edc1612c78ff6f481363ae54ae6d377))
* Json-compatible pkg data object ([17d6673](https://github.com/twistapps/twistcore/commit/17d6673f5e8d73610eb5f221e42ff4cf59237dfe))
* Package creation ([d057706](https://github.com/twistapps/twistcore/commit/d057706fbf0d123e5e809105ce398ee774a74b53))

# [0.3.0](https://github.com/twistapps/twistcore/compare/0.2.1...0.3.0) (2022-10-22)


### Features

* Compare local pkg version with GitHub ([a225e40](https://github.com/twistapps/twistcore/commit/a225e40a9584cc5d9466ae2526953f9b50298c8d))
* UPM interface and utils ([099cffd](https://github.com/twistapps/twistcore/commit/099cffd3dd44b7751b2855508d1477d602206f47))

## [0.2.1](https://github.com/twistapps/twistcore/compare/0.2.0...0.2.1) (2022-10-22)


### Bug Fixes

* namespaces/using ([9be24c9](https://github.com/twistapps/twistcore/commit/9be24c9a732fd28bb12e93efd1844ae800d552de))

# [0.2.0](https://github.com/twistapps/twistcore/compare/0.1.0...0.2.0) (2022-10-22)


### Features

* CodeGen save to any file ([f7c9608](https://github.com/twistapps/twistcore/commit/f7c9608c09a6e44fbfc7645b1dd822c8214ecfb9))
* CodeGen variables QoL ([6b18618](https://github.com/twistapps/twistcore/commit/6b18618bcc56ee6f187b42fe5661b85c61fff48b))

# [0.1.0](https://github.com/twistapps/twistcore/compare/0.0.2...0.1.0) (2022-10-22)


### Features

* Core definitions ([eadb4c2](https://github.com/twistapps/twistcore/commit/eadb4c23ddc995c81ee6140a309c2d9e4b017c56))
* Core Settings asset ([1ffcb6d](https://github.com/twistapps/twistcore/commit/1ffcb6d1747d06ac9d7fcc01d1b2bcbb2d965e83))
* Create commonly used GUIStyles ([9ab1bd9](https://github.com/twistapps/twistcore/commit/9ab1bd9f7a659c9b2e061e66f90305a6102fcfc9))
* Embed CodeGen package ([9ee7bfa](https://github.com/twistapps/twistcore/commit/9ee7bfa127014bad1c1e5112c19ffc7f24753ef1))
* Git commands interface ([b17fe8e](https://github.com/twistapps/twistcore/commit/b17fe8e8df49e1065e54802b6f0c1d05fd7e71d0))
* Package versioning utils ([77451b3](https://github.com/twistapps/twistcore/commit/77451b31df80db0c1f05ece435ebc507fbcfcfac))

## [0.0.2](https://github.com/twistapps/twistcore/compare/0.0.1...0.0.2) (2022-10-22)


### Bug Fixes

* release versioning ([7c9502c](https://github.com/twistapps/twistcore/commit/7c9502c7654101977f159b1bdfaa1a1bb191c0c1))

# 1.0.0 (2022-10-22)


### Features

* First Release ([447c9e2](https://github.com/twistapps/twistcore/commit/447c9e235799dbc20d759cdff7df133eb994d563))
* Release Workflow ([8c0dbea](https://github.com/twistapps/twistcore/commit/8c0dbea1ea4287a2b7a5e2e82870efdc4ab94074))
