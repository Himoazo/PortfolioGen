# PortfolioGen

- Check Github api, GET is simple but explore OAuth with Github
- Some Views has IEnumerable while they shouldn't. Fix return array in view where it shouldn't
- Error handling specially NotFound and Unauthorized. Make custom 404 notfound page. Explore ErrorViewModel and its possibilities
- You had a note about adding Unique constraint to Project table (not sure what it means check it anyway)
- Hide Create portfolio button when there's one and manage to not let the app crash if somehow a user manage to send more create requests when the user 
has a portfolio (SqliteException: SQLite Error 19: 'UNIQUE constraint failed: Portfolios.AppUserId'.)





# Fore report and video
Don't forget to mention:
- It's not a portfolio website, it's a CMS which generates portfolios till alla
- app.MapFallbackToController("Profile", "Public"); id = HttpContext.Request.Path.Value?.TrimStart('/');
Extra features:
- Image upload and auto convert and resize
- Github API integration
- ? Dark/Light theme mode
- ? Multi languages
