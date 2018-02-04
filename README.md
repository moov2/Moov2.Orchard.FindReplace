# Moov2.Orchard.FindReplace

[Orchard](http://www.orchardproject.net/) module for performing find & replace operation for content within content items.

## Getting Set Up

Download module source code and place within the "Modules" directory of your Orchard installation.

Alternatively, use the command below to add this module as a sub-module within your Orchard project.

    git submodule add git@github.com:moov2/Moov2.Orchard.FindReplace.git modules/Moov2.Orchard.FindReplace

# Usage

Enable the "Moov2.Orchard.FindReplace" module. 

Once enabled, admin users will be able to see a "Find & Replace" menu within the admin dashboard. By default only `Admin` users will be authorised, however permission can be granted to other roles by giving access to `Access Find/Replace" permission.

Navigating to the Find & Replace section will present the user with a text box to input their term that they would like to replace. Submitting this form will present all the content items that contain the term. By default all content items will be checked, meaning they will be included in the replacement. If the content item is unchecked then it will be excluded from having the term replaced. Above the list of content items is another text box for entering the replacement term. Leaving this blank will remove the term and replace with nothing.

Once submitting the form the user will be redirected back to the first form with a success message. All items affected by the update will have their modified date / modified by updated.