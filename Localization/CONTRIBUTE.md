# How to contribute translations?
- Fork clone the [localization](https://github.com/ChrisFeline/ToNSaveManager/tree/localization) branch.
- Create a copy of the `en-US.json` language file.
- Rename it to your local ISO language name. For example `ja-JP.json`
- Translate the strings contained within this file into your target language.
	* Keep important string replacement tokens like: `{0}`, `{1}` or `$$MAIN.SETTINGS$$` etc...
- Create a pull request.
	* Do **NOT** create a pull request into the `main` branch.
	* Make sure the only edited file is the new added language `.json` file, any other contribution in the source code unrelated to this translation will be rejected.