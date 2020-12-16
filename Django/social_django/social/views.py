from django.shortcuts import render

def feed(request):
    return render(request, 'social/feed.html')

def profile(request):
    return render(request, 'social/profile.html')
