//-- Some default colors to work with
var EditPolylineViewOptions = null;
var EditPolygonViewOptions = null;

function RenderDefaultCfBing7Map(id, e) {
    return RenderDefaultCfBing7Map(id, e, Microsoft.Maps.MapTypeId.road);
}

function RenderDefaultCfBing7Map(id, e, mapTypeId) {
    EditPolylineViewOptions = { fillColor: new Microsoft.Maps.Color(56, 0, 0, 255) };
    EditPolygonViewOptions = { strokeThickness: 2, strokeColor: new Microsoft.Maps.Color(156, 0, 255, 0), fillColor: new Microsoft.Maps.Color(56, 0, 0, 255) };
    
    var mOpt = new Object();
    mOpt.credentials = e;
    mOpt.mapTypeId = mapTypeId;
    mOpt.enableClickableLogo = false;
    mOpt.enableSearchLogo = false;
    mOpt.showLogo = false;
    mOpt.showCopyright = false;
    mOpt.padding= 1;

    var mapWithID = new Microsoft.Maps.Map(document.getElementById(id), mOpt);

    Microsoft.Maps.registerModule("CustomInfoboxModule", "http://static.climbfind.com/js/cf.mapInfoBox.js");
    Microsoft.Maps.loadModule("CustomInfoboxModule", { callback: function () { pinInfobox = new CustomInfobox(mapWithID); } });
    
    return mapWithID;
}

function RenderLatLongEditorCfBing7Map(id, e) {
    var mOpt = new Object();
    mOpt.credentials = e;
    mOpt.mapTypeId = Microsoft.Maps.MapTypeId.road;
    mOpt.enableClickableLogo = false;
    mOpt.enableSearchLogo = false;
    mOpt.showLogo = false;
    mOpt.showCopyright = false;
    mOpt.padding= 1;
    
    var mapWithID = new Microsoft.Maps.Map(document.getElementById(id), mOpt);
    return mapWithID;
}

var GeoJsonLayer = function (id, map, iconImgUrlRoot, infoImgUrlRoot, polylineOptions, polygonOptions) {
    var _mapID = id,
        _map = map,
        _iconImgUrlRoot = iconImgUrlRoot,
        _infoImgUrlRoot = infoImgUrlRoot,
        _callback = null,
        _polylineOptions = polylineOptions,
        _polygonOptions = polygonOptions,
        _allCoords = [],
        _customView = null;

    /*****************
    * Private Methods
    ******************/
    function parseGeoJson(geoJson) {
        var items = new Microsoft.Maps.EntityCollection();

        var geoItemsArray = geoJson.Items;
        var customView = geoJson.CustomView;
        if (customView != null) { _customView = customView; }

        for (i = 0; i < geoItemsArray.length; i++) {
            var item = geoItemsArray[i];
            var shape = parseGeoJsonItem(item);
            if (shape != null) {
                items.push(shape);
            }
        }

        if (_callback != null) {
            _callback(items);
        }
    }

    this.AddJsonMapItem = function (layerToAddTo, json) {
        var shape = parseGeoJsonItem(json);
        layerToAddTo.push(shape);
    }

    function parseGeoJsonItem(i) {
        var shape = null;
        var shapeCoord = i.C, title = i.T, description = i.D, link = i.L, geoType = i.GT, type = i.CT, img = i.I;
        var icon;

        switch (geoType) {
            case "Point":
                tempCoord = parseCoord(shapeCoord);
                if (tempCoord != null) {
                    _allCoords.push(tempCoord);
                    var iconImg, anch;

                    iconImg = _iconImgUrlRoot + '/' + type + '.png';

                    var displayImgUrl;
                    //-- if it has an extension it's custom image set by a user
                    var hasFileExtension = (img.indexOf('.') != -1);
                    if (hasFileExtension) {
                        var folder = "/ar/";
                        if (type == "id") { folder = "/id/"; }
                        if (type == "od") { folder = "/od/"; }
                        displayImgUrl = _infoImgUrlRoot + folder + img;
                    }
                    else { displayImgUrl = _iconImgUrlRoot + '/d' + type + '.png'; }

                    shape = new Microsoft.Maps.Pushpin(tempCoord, { icon: iconImg, height: '22px', anchor: new Microsoft.Maps.Point(9, 20) });

                    infoBoxContents[tempCoord] = "<a href='" + link + "'><b>" + title + "</b><img src='" + displayImgUrl + "' /></a>";

                    Microsoft.Maps.Events.addHandler(shape, 'mouseover', pinMouseOver);
                    //Microsoft.Maps.Events.addHandler(shape, 'mouseout', pinMouseOut);
                }
                break;
            case "Line":
                tempCoord = parseCoords(shapeCoord, 2);
                if (tempCoord != null && tempCoord.length >= 2) {
                    _allCoords = _allCoords.concat(tempCoord);
                    shape = new Microsoft.Maps.Polyline(tempCoord, _polylineOptions);
                }
                break;
            case "Polygon":
                tempCoord = parseCoords(shapeCoord, 2);
                if (tempCoord != null && tempCoord.length >= 3) {
                    _allCoords = _allCoords.concat(tempCoord);
                    shape = new Microsoft.Maps.Polygon(tempCoord, _polygonOptions);
                }
                break;
            case "georss:circle":
                alert('circle not yet supported');
                //                    var v = $.trim($(this).text().replace(/,/g, ' ').replace(/[\s]{2,}/g, ' ')).split(' ');
                //                    if (v.length > 2) {
                //                        tempCoord = geoTools.GenerateRegularPolygon(new Microsoft.Maps.Location(parseFloat(v[0]), parseFloat(v[1])), parseFloat(v[2]), geoTools.Constants.EARTH_RADIUS_METERS, 25, 0);
                //                        _allCoords = _allCoords.concat(tempCoord);
                //                        shape = new Microsoft.Maps.Polygon(tempCoord, _polygonOptions);
                //                    }
                break;
            default:
                alert(geoType + ' map item type not suported');
                break;
        }

        return shape;
    }

    /*
    * Parses a string list of coordinates. Handles 2D and 3D coordinate sets.
    * dim - number of values to represent coordinate. 
    */
    function parseCoords(sCoord, dim) {
        if (dim == null || dim < 1) {
            dim = 2;
        }

        var v = $.trim(sCoord.replace(/,/g, ' ').replace(/[\s]{2,}/g, ' ')).split(' ');
        if (v.length > 1) {
            var c = [];

            for (var i = 0; i < v.length; i = i + dim) {
                c.push(new Microsoft.Maps.Location(parseFloat(v[i]), parseFloat(v[i + 1])));
            }

            return c;
        }

        return null;
    }

    /* Parses a string list of coordinate */
    function parseCoord(sCoord) {
        var v = $.trim(sCoord.replace(/,/g, ' ').replace(/[\s]{2,}/g, ' ')).split(' ');
        if (v.length > 1) {
            return new Microsoft.Maps.Location(parseFloat(v[0]), parseFloat(v[1]));
        }

        return null;
    }

    /****************
    * Public Methods
    ****************/

    this.LoadGeo = function (link, callback) {
        _callback = callback;
        _bounds = new Microsoft.Maps.LocationRect(new Microsoft.Maps.Location(0, 0), 360, 180);

        $.ajax({
            type: "POST",
            url: link,
            dataType: "json",
            success: function (json) { parseGeoJson(json); },
            error: function (xhr, ajaxOptions, thrownError) { alert(thrownError.description); }
        });

    };


    this.SetCustomView = function () {
        if (_customView != null) {
            var centerLoc = new Microsoft.Maps.Location(_customView.Lat, _customView.Long);
            var mapZoom = parseInt(_customView.Zoom);

            _map.setView({ center: centerLoc, zoom: mapZoom });
        }
        else {
            var layerBounds;

            if (_allCoords != null && _allCoords.length > 0) {
                layerBounds = Microsoft.Maps.LocationRect.fromLocations(_allCoords);
            }
            _map.setView({ bounds: layerBounds });
        }
    };

    this.GetBounds = function () {
        if (_allCoords != null && _allCoords.length > 0) {
            return Microsoft.Maps.LocationRect.fromLocations(_allCoords);
        }

        return null;
    };
}

/* Info box shit */

function displayInfobox(e) {
    var pin = e.target;
    if (pin != null) {
        var location = pin.getLocation();
       

        //Display Infobox
        pinInfobox.show(location, "<span class='mapInfoBox'>" + infoBoxContents[location] + "</span>");
    }
}

function pinMouseOver(e) { displayInfobox(e); }
function pinInfoboxMouseEnter(e) { stopInfoboxTimer(e); }

/* End info box shit */



function GetPolygonBoundingLocationRect(_allCoords) {
if (_allCoords != null && _allCoords.length > 0) {
return Microsoft.Maps.LocationRect.fromLocations(_allCoords);
}

return null;
};

/*
* Parses a string list of coordinates. Handles 2D and 3D coordinate sets.
* dim - number of values to represent coordinate. 
*/
function parse2dPolygonCoords(sCoord) {
    dim = 2;
    
    var v = $.trim(sCoord.replace(/,/g, ' ').replace(/[\s]{2,}/g, ' ')).split(' ');
    if (v.length > 1) {
        var c = [];

        for (var i = 0; i < v.length; i = i + dim) {
            c.push(new Microsoft.Maps.Location(parseFloat(v[i]), parseFloat(v[i + 1])));
        }

        return c;
    }

    return null;
}

function SetDraggablePin(entityCollection) {
    var pushpinLayer = new Microsoft.Maps.EntityCollection();

    // Retrieve the location of the map center 
    var center = map.getCenter();

    // Add a pin to the center of the map
    pin = new Microsoft.Maps.Pushpin(center, { text: '1', draggable: true });

    alert(pin);

    pushpinLayer.push(pin);
    entityCollection.push(pushpinLayer);
}

var resizeByCornersMap;
var resizeByCornersPolygonLayer;
var resizeByCornersCornersLayer;
var resizeByCornersWktInput;
var resizeByCornersOriginalBox;
var resizeByCornersCurrentBox;
var resizeByCornersCenterLocation;
var resizeByCornersPinNW;
var resizeByCornersPinSE;
var resizeByCornersBoxValid;
var resizeByCornersRezoomMapAfterEdit;

function ClearResizeCornersFromBoundingBox() {
    if (resizeByCornersCornersLayer != null) { resizeByCornersCornersLayer.clear(); }
    
    resizeByCornersMap = null;
    resizeByCornersPolygonLayer = null;
    resizeByCornersCornersLayer = null;
    resizeByCornersWktInput = null;
    resizeByCornersOriginalBox = null;
    resizeByCornersCurrentBox = null;
    resizeByCornersCenterLocation = null;
    resizeByCornersPinNW = null;
    resizeByCornersPinSE = null;
    resizeByCornersBoxValid = true;
    resizeByCornersRezoomMapAfterEdit = true;
}

function AddResizeCornersToBoundingBox(map, boxLayer, wktInput, originalBoxLocationRect, originalBoxCenterLocation) 
{
    ClearResizeCornersFromBoundingBox();

    resizeByCornersMap = map;
    resizeByCornersPolygonLayer = boxLayer;
    resizeByCornersWktInput = wktInput;
    resizeByCornersWktInput.val(BuildWKTFromLocationRect(originalBoxLocationRect));
    resizeByCornersOriginalBox = originalBoxLocationRect;

    if (resizeByCornersCornersLayer == null) {      
        resizeByCornersCornersLayer = new Microsoft.Maps.EntityCollection();
        map.entities.push(resizeByCornersCornersLayer);
    }

    var b = originalBoxLocationRect;

    resizeByCornersPinNW = new Microsoft.Maps.Pushpin(b.getNorthwest(), { text: 'NW', draggable: true });
    resizeByCornersPinSE = new Microsoft.Maps.Pushpin(b.getSoutheast(), { text: 'SE', draggable: true });
   
    resizeByCornersCornersLayer.clear();
    resizeByCornersCornersLayer.push(resizeByCornersPinNW);
    resizeByCornersCornersLayer.push(resizeByCornersPinSE);

    if (originalBoxCenterLocation != null) {
        resizeByCornersCenterLocation = originalBoxCenterLocation;
        resizeByCornersPinCenter = new Microsoft.Maps.Pushpin(resizeByCornersCenterLocation, { text: 'C' });
        resizeByCornersCornersLayer.push(resizeByCornersPinCenter);
    }

    //Wire events for dragging
    Microsoft.Maps.Events.addHandler(resizeByCornersPinNW, 'drag', DrawResizedBoundingBoxPolygon, 100);
    Microsoft.Maps.Events.addHandler(resizeByCornersPinSE, 'drag', DrawResizedBoundingBoxPolygon, 100);
    Microsoft.Maps.Events.addHandler(resizeByCornersPinNW, 'dragend', TrySetBoundingBoxPolygon);
    Microsoft.Maps.Events.addHandler(resizeByCornersPinSE, 'dragend', TrySetBoundingBoxPolygon);
}

function DrawResizedBoundingBoxPolygon(e) {
    resizeByCornersCurrentBox = Microsoft.Maps.LocationRect.fromCorners(resizeByCornersPinNW.getLocation(), resizeByCornersPinSE.getLocation());
    RedrawResizeCornersBoundingBox();
}

function RedrawResizeCornersBoundingBox(e) {
    resizeByCornersPolygonLayer.clear();
    var polygonPoints = GetPolygonPointsFromLocationRect(resizeByCornersCurrentBox);
    var polygon = new Microsoft.Maps.Polygon(polygonPoints, EditPolygonViewOptions);

    resizeByCornersPolygonLayer.push(polygon);
}

function TrySetBoundingBoxPolygon(e) {
    var locNW = resizeByCornersPinNW.getLocation();
    var locSE = resizeByCornersPinSE.getLocation();

    //-- If we have a center point we restrict the corners from going over the center, otherwise we restrict them from going over each other
    if (resizeByCornersCenterLocation != null) {
        if ((resizeByCornersBoxValid == true) && (locNW.latitude < resizeByCornersCenterLocation.latitude)) { SetInvalidResizedBoundingBoxPolygon('North West corner must be north of the center point.'); }
        if ((resizeByCornersBoxValid == true) && (locNW.longitude > resizeByCornersCenterLocation.longitude)) { SetInvalidResizedBoundingBoxPolygon('North West corner must be west of the center point.'); }
        if ((resizeByCornersBoxValid == true) && (locSE.latitude > resizeByCornersCenterLocation.latitude)) { SetInvalidResizedBoundingBoxPolygon('South East corner must be south of the center point.'); }
        if ((resizeByCornersBoxValid == true) && (locSE.longitude < resizeByCornersCenterLocation.longitude)) { SetInvalidResizedBoundingBoxPolygon('South East corner must be east of the center point.'); }
    } else {
        if ((resizeByCornersBoxValid == true) && (locNW.latitude < locSE.latitude)) { SetInvalidResizedBoundingBoxPolygon('North West corner must be north of south east corner.'); }
        if ((resizeByCornersBoxValid == true) && (locNW.longitude > locSE.longitude)) { SetInvalidResizedBoundingBoxPolygon('North West corner must be west of south east corner.'); }
    }

    if (resizeByCornersBoxValid != true) 
    {
        resizeByCornersCurrentBox = resizeByCornersOriginalBox;
        resizeByCornersPinNW.setLocation(resizeByCornersOriginalBox.getNorthwest());
        resizeByCornersPinSE.setLocation(resizeByCornersOriginalBox.getSoutheast());
        resizeByCornersBoxValid = true;
    }

    RedrawResizeCornersBoundingBox();
      
    if (resizeByCornersRezoomMapAfterEdit) {
        resizeByCornersMap.setView({ bounder: resizeByCornersCurrentBox });
    }

    var wkt = BuildWKTFromLocationRect(resizeByCornersCurrentBox);
    resizeByCornersWktInput.val(wkt);
}

function SetInvalidResizedBoundingBoxPolygon(msg) {
    resizeByCornersBoxValid = false;
    alert(msg);
}


var addNavButton = function (mapElement, buttonID, content, onclick) {
    var navContainer = $(mapElement).find('.NavBar_typeButtonContainer');
    navContainer.append($('<span>').addClass('NavBar_separator')).append(
                $('<a>').attr('id', buttonID).attr('href', '#').addClass('NavBar_button').append($('<span>').html(content).click(onclick)));
};

var geoTools = new function () {
    function DegtoRad(x) {
        return x * Math.PI / 180;
    }

    function RadtoDeg(x) {
        return x * 180 / Math.PI;
    }

    this.Constants = {
        EARTH_RADIUS_METERS: 6378100,
        EARTH_RADIUS_KM: 6378.1,
        EARTH_RADIUS_MILES: 3963.1676,
        EARTH_RADIUS_FEET: 20925524.9
    };

    this.CalculateCoord = function (origin, brng, arcLength, earthRadius) {
        var lat1 = DegtoRad(origin.latitude),
            lon1 = DegtoRad(origin.longitude),
            centralAngle = arcLength / earthRadius;

        var lat2 = Math.asin(Math.sin(lat1) * Math.cos(centralAngle) + Math.cos(lat1) * Math.sin(centralAngle) * Math.cos(DegtoRad(brng)));
        var lon2 = lon1 + Math.atan2(Math.sin(DegtoRad(brng)) * Math.sin(centralAngle) * Math.cos(lat1), Math.cos(centralAngle) - Math.sin(lat1) * Math.sin(lat2));

        return new Microsoft.Maps.Location(RadtoDeg(lat2), RadtoDeg(lon2));
    };

    this.GenerateRegularPolygon = function (centerPoint, radius, earthRadius, numberOfPoints, offset) {
        var points = [],
            centralAngle = 360 / numberOfPoints;

        for (var i = 0; i <= numberOfPoints; i++) {
            points.push(geoTools.CalculateCoord(centerPoint, (i * centralAngle + offset) % 360, radius, earthRadius));
        }

        return points;
    };

    this.HaversineDistance = function (coord1, coord2, earthRadius) {
        var lat1 = DegtoRad(coord1.latitude),
            lon1 = DegtoRad(coord1.longitude),
            lat2 = DegtoRad(coord2.latitude),
            lon2 = DegtoRad(coord2.longitude);

        var dLat = lat2 - lat1,
            dLon = lon2 - lon1,
            cordLength = Math.pow(Math.sin(dLat / 2), 2) + Math.cos(lat1) * Math.cos(lat2) * Math.pow(Math.sin(dLon / 2), 2),
            centralAngle = 2 * Math.atan2(Math.sqrt(cordLength), Math.sqrt(1 - cordLength));

        return earthRadius * centralAngle;
    };
};

function GetPolygonPointsFromLocationRect(locationRect) {
    var bb = locationRect;

    polygonPoints = [new Microsoft.Maps.Location(bb.getSouth(), bb.getWest()),
                            new Microsoft.Maps.Location(bb.getSouth(), bb.getEast()),
                            new Microsoft.Maps.Location(bb.getNorth(), bb.getEast()),
                            new Microsoft.Maps.Location(bb.getNorth(), bb.getWest()),
                            new Microsoft.Maps.Location(bb.getSouth(), bb.getWest())];

    return polygonPoints;
}

function BuildWKTFromLocationRect(locationRect) {
    var bb = locationRect;
    var polygonWKT = 'POLYGON((' + bb.getWest().toFixed(4) + ' ' + bb.getSouth().toFixed(4) + ', ' +
                                bb.getEast().toFixed(4) + ' ' + bb.getSouth().toFixed(4) + ', ' +
                                bb.getEast().toFixed(4) + ' ' + bb.getNorth().toFixed(4) + ', ' +
                                bb.getWest().toFixed(4) + ' ' + bb.getNorth().toFixed(4) + ', ' +
                                bb.getWest().toFixed(4) + ' ' + bb.getSouth().toFixed(4) + '))';
    return polygonWKT;
}


//function RenderFreshPolygon(map, polygonPoints) {
//    map.entities.clear();

//    var polygon = new Microsoft.Maps.Polygon(polygonPoints,
//                { strokeThickness: 2, strokeColor: new Microsoft.Maps.Color(156, 0, 255, 0), fillColor: new Microsoft.Maps.Color(56, 0, 0, 255) });

//    map.entities.push(polygon);

//    var boundingCorners = Microsoft.Maps.LocationRect.fromLocations(polygonPoints.slice(1));

//    map.setView({ bounds: boundingCorners });
//}