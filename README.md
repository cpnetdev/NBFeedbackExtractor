# NBFeedbackExtractor
NBFeedbackExtractor is a .NET 4.5 WinForms application that extracts all feedback activities (comments) from a NationBuilder Feedback page.  NationBuilder does not support exporting of feedback, and their current workaround ([http://nationbuilder.com/how_do_i_print_a_csv_of_feedback](http://nationbuilder.com/how_do_i_print_a_csv_of_feedback)) is not very practical if you have many pages of feedback.

There's lots of room for improvement in this code if you want to modify it.  This is just something quick and dirty I threw together to get some comments.  This code essentially implements the workaround at [http://nationbuilder.com/how_do_i_print_a_csv_of_feedback](http://nationbuilder.com/how_do_i_print_a_csv_of_feedback) except it automtically concatenates every page of feedback available in NationBuilder and converts it all into a single .csv.

To run the application, you can grab the files in https://github.com/cpnetdev/NBFeedbackExtractor/tree/master/NBFeedbackExtractor/Binaries copy them to your local machine, and double-click on "NBFeedbackExtractor.exe" to run it.

You'll need to be on a Windows machine with .NET 4.5 (or newer) installed.  If you've been running all critical and recommended Windows updates, you should have this.
