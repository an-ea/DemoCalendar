let currentEvent;
const dateFormat = "DD.MM.YYYY HH:mm";
const formatDate = date => date === null ? '' : moment(date).format(dateFormat);
const fpStartTime = flatpickr("#StartTime", {
    enableTime: true,
    time_24hr: true,
    dateFormat: "d.m.Y H:i"
});
const fpEndTime = flatpickr("#EndTime", {
    enableTime: true,
    time_24hr: true,
    dateFormat: "d.m.Y H:i"
});

$('#calendar').fullCalendar({
    timeZone: 'UTC',
    timeFormat: 'HH:mm',
    defaultView: 'month',
    height: 'parent',
    header: {
        left: 'prev,next today',
        center: 'title',
        right: ''
    },
    eventRender(event, $el) {
        $el.qtip({
            content: {
                title: event.title,
                text: event.description
            },
            hide: {
                event: 'unfocus'
            },
            show: {
                solo: true
            },
            position: {
                my: 'top left',
                at: 'bottom left',
                viewport: $('#calendar-wrapper'),
                adjust: {
                    method: 'shift'
                }
            }
        });
    },
    events: '/Home/GetEvents',
    eventClick: updateEvent,
    selectable: true,
    select: addEvent
});


function updateEvent(event, element) {
    currentEvent = event;

    if ($(this).data("qtip")) $(this).qtip("hide");
    
    $('#eventModalLabel').html('Edit Event');
    $('#eventModalSave').html('Update Event');
    $('#EventTitle').val(event.title);
    $('#Description').val(event.description);
    $('#isNewEvent').val(false);
    $('#deleteEvent').show();

    const start = formatDate(event.start);
    const end = formatDate(event.end);

    fpStartTime.setDate(start);
    fpEndTime.setDate(end);

    $('#StartTime').val(start);
    $('#EndTime').val(end);

    $('#eventModal').modal('show');
}

function addEvent(start, end) {
    $('#eventForm')[0].reset();

    $('#eventModalLabel').html('Add Event');
    $('#eventModalSave').html('Create Event');
    $('#isNewEvent').val(true);
    $('#deleteEvent').hide();

    start = moment(start).add(12, 'hours');
    end = moment(start).add(30, 'minutes');

    start = formatDate(start);
    end = formatDate(end);

    fpStartTime.setDate(start);
    fpEndTime.setDate(end);

    $('#eventModal').modal('show');
}


$('#eventModalSave').click(() => {
    const title = $('#EventTitle').val();
    const description = $('#Description').val();
    const startTime = moment.utc($('#StartTime').val(), dateFormat);
    const endTime = moment.utc($('#EndTime').val(), dateFormat);
    const isNewEvent = $('#isNewEvent').val() === 'true';

    if (title.trim().length === 0) {
        alert('Please enter the Title');
        return;
    }
    if (startTime >= endTime) {
        alert('Start Time cannot be greater than or equal to End Time');
        return;
    }
    if ((!startTime.isValid() || !endTime.isValid())) {
        alert('Please enter both Start Time and End Time');
        return;
    }
    if (description.trim().length === 0) {
        alert('Please enter the Description');
        return;
    }

    const event = {
        title,
        description,
        startTime: startTime,
        endTime: endTime
    };

    if (isNewEvent) {
        sendAddEvent(event);
    } else {
        sendUpdateEvent(event);
    }
});

function sendAddEvent(event) {
    axios({
        method: 'post',
        url: '/Home/AddEvent',
        data: {
            "Title": event.title,
            "Description": event.description,
            "Start": event.startTime,
            "End": event.endTime
        }
    })
    .then(res => {
        const { message, eventId } = res.data;

        if (message === '') {
            const newEvent = {
                start: event.startTime,
                end: event.endTime,
                title: event.title,
                description: event.description,
                eventId
            };

            $('#calendar').fullCalendar('renderEvent', newEvent);
            $('#calendar').fullCalendar('unselect');
            
            $('#eventModal').modal('hide');
        } else {
            alert(`Something went wrong: ${message}`);
        }
    })
    .catch(err => alert(`Something went wrong: ${err}`));
}

function sendUpdateEvent(event) {
    axios({
        method: 'put',
        url: '/Home/UpdateEvent',
        data: {
            "EventId": currentEvent.eventId,
            "Title": event.title,
            "Description": event.description,
            "Start": event.startTime,
            "End": event.endTime
        }
    })
    .then(res => {
        const { message } = res.data;

        if (message === '') {
            currentEvent.title = event.title;
            currentEvent.description = event.description;
            currentEvent.start = event.startTime;
            currentEvent.end = event.endTime;

            $('#calendar').fullCalendar('updateEvent', currentEvent);
            $('#eventModal').modal('hide');
        } else {
            alert(`Something went wrong: ${message}`);
        }
    })
    .catch(err => alert(`Something went wrong: ${err}`));
}

$('#deleteEvent').click(() => {
    if (confirm(`Delete "${currentEvent.title}" event?`)) {
        axios({
            method: 'delete',
            url: '/Home/DeleteEvent',
            data: currentEvent.eventId,
            headers: { 'Content-Type': 'application/json; charset=utf-8' }
        })
        .then(res => {
            const { message } = res.data;

            if (message === '') {
                $('#calendar').fullCalendar('removeEvents', currentEvent._id);
                $('#eventModal').modal('hide');
            } else {
                alert(`Something went wrong: ${message}`);
            }
        })
        .catch(err => alert(`Something went wrong: ${err}`));
    }
});
