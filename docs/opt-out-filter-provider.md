# An IFilterProvider using Opt-Out thinking

When it comes to securing a website your worst enemies are your own developers.  Any one of them can forget to secure an entry point and as projects get bigger it becomes very difficult to track down these open doors.  I have come to hate annotation as the way to secure actions in controllers.  I have gone so far as to search for all **[Authorize]** annotations in code and remove them, thus forcing the deveopers to implement security another way.

# The Opt-Out way
The Opt-Out way states that you are automatically opted into security unless you Opt-Out.  When you Opt-Out you must configure your way out of security.  This leaves an audit trail which can then more easily be reviewed.

To achieve this an application needs to find all controllers of interest at startup and impose security filters.  Now no single developer is responsible for their individual controller.  For applications where everything requires a user to be logged in, the only controllers that need to Opt-Out are usually the cold landing pages. 

```
"OptOut": [
    {
       "Filter": "P7.Filters.AuthActionFilter,P7.Filters",
       "RouteTree": [
           {
               "Area": "",
               "Description": "Every action with no area is opted out of this Filter"
           },
           {
              "Area": "Main",
              "Description": "Every action within the Main area is opted out of this Filter"
           },
           {
              "Area": "Identity",
              "Description": "Every action within the Identity area is opted out of this Filter"

           },
           {
              "Area": "Sports",
              "Description": "Not everything in the Sports area is opted out of this Filter"
              "Controllers": [
                  {
                     "Description": "/Sports/Work/Open is opted out of this Filter",
                     "Controller": "Work",
                     "Actions": [
                       { "Action": "Open" }
                     ]
                  },
                 {
                     "Description": "Everything in the /Sports/Home controller is opted out of this Filter",
                     "Controller": "Home"
                 }
              ]
           }
       ]
    }
]
```

In P7 an example IFilterProvider is implemented that uses a local json configuration as its database.  
Conversely I have also implemented an Opt-In configuration that provides the more traditional way of assigning filters.  That is there for areas that Opt-Out of a certain filter, but need to Opt-In to a more specialized filter.  I still get an audit trail as to why someone chose to Opt-Out of the original filter.

