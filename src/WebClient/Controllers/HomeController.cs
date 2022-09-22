using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebClient.Exceptions;
using WebClient.Models;
using WebClient.Services;

namespace WebClient.Controllers;


[Authorize]
public class HomeController : Controller
{
    private readonly ICalendarService _calendarService;
    private readonly IMapper _mapper;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ICalendarService calendarService, IMapper mapper, ILogger<HomeController> logger)
    {
        _calendarService = calendarService ?? throw new ArgumentNullException(nameof(calendarService));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IActionResult Index() => View();

    public IActionResult Privacy() => View();



    [HttpGet]
    public async Task<IActionResult> GetEventsAsync(string start, string end)
    {
        _logger.LogInformation("Getting events in the range '{start}' - '{end}'.", start, end);

        static DateOnly Parse(string value) => DateOnly.Parse(value);
        var events = await _calendarService.FindEventsAsync(Parse(start), Parse(end));

        var models = _mapper.Map<Event[]>(events);

        _logger.LogInformation("Found {count} events.", models.Length);

        return Json(models);
    }


    [HttpPost]
    public async Task<IActionResult> AddEventAsync([FromBody]NewEvent newEvent)
    {
        if (!ModelState.IsValid)
            return CreateResultWithMessage(ModelState.Values.ToString()!);

        _logger.LogInformation("Adding the event: title - {title}, description - {description}, {start} - {end}", newEvent.Title, newEvent.Description, newEvent.Start, newEvent.End);

        var dto = _mapper.Map<NewEventDto>(newEvent);

        try
        {
            var info = await _calendarService.CreateEventAsync(dto);
            return Json(new { message = string.Empty, eventId = info.Id });
        }
        catch (EventExistsException)
        {
            return CreateEventExistsResult();
        }
    }


    [HttpPut]
    public async Task<IActionResult> UpdateEvent([FromBody] Event @event)
    {
        if (!ModelState.IsValid)
            return CreateResultWithMessage(ModelState.Values.ToString()!);

        _logger.LogInformation("Updating the event: id - {id}", @event.EventId);

        var eventDto = _mapper.Map<EventDto>(@event);

        try
        {
            await _calendarService.UpdateEventAsync(eventDto);
            return CreateResultWithMessage();
        }
        catch (EventExistsException)
        {
            return CreateEventExistsResult();
        }
    }



    [HttpDelete]
    public async Task<IActionResult> DeleteEventAsync([FromBody]int id)
    {
        _logger.LogInformation("Deleting the event: id - {id}", id);

        await _calendarService.DeleteEventAsync(id);
        return CreateResultWithMessage();
    }




    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private JsonResult CreateResultWithMessage(string message = "") => Json(new { message });

    private JsonResult CreateEventExistsResult() => CreateResultWithMessage("Event already exists");
}