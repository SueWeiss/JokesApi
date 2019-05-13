$(() => {
    alert('foo');
    

    $('.likeBtn').on('click', function () {
        const JokeId = $(this).data('id');
        $.post("/home/addLike", { Id: JokeId }, function (message) {
            alert(message)
            console.log('added')
        })
    })


});