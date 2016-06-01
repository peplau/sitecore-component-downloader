var myListPageDialog;

define(["sitecore"], function (Sitecore) {
    var ListPageDialog = Sitecore.Definitions.App.extend({
        initialized: function () {
            alert(123);
            debugger;

            myListPageDialog = this;

            // Expand all nodes
            //this.expandAllTreeViews();

            ////var treeView = $("." + this.TreeDsTemplate.attributes.type);

            //var rootInfo = "Alert,master,df686a84-eec4-4b13-8121-f9ab343f3a0f,/temp/IconCache/Network/16x16/home.png";
            //var rootPath = "/sitecore/layout/Sublayouts/Keystone/Components/Alert";

            //this.TreeDsTemplate.set("rootitem", "{df686a84-eec4-4b13-8121-f9ab343f3a0f}");
            //this.TreeDsTemplate.set("database", "master");

            ////treeView.attr('data-sc-rootitem', "df686a84-eec4-4b13-8121-f9ab343f3a0f");
            ////treeView.attr('data-sc-database', "master");

            ////this.TreeDsTemplate.viewModel.getRoot().removeChildren();
            ////this.TreeDsTemplate.viewModel.checkedItemIds([]);
            ////this.TreeDsTemplate.viewModel.initialized();

            //return;

            //var model = Sitecore.Definitions.Models.ControlModel.extend({
            //    initialize: function (options) {
            //        alert(456);
            //        debugger;
            //        this._super();
            //    }
            //});

            //var view = Sitecore.Definitions.Views.ControlView.extend({
            //    initialize: function (options) {
            //        alert(789);
            //        debugger;
            //        this._super();
            //    }
            //});

            //Sitecore.Speak.Factories.createComponent("ItemTreeViews", model, view, ".sc-SimpleComponent");

        },
        visitAllTreeViews: function(callback) {
            this.visitTreeView(this.TreeDsTemplate, callback);
            this.visitTreeView(this.TreeDsBaseTemplates, callback);
        },
        visitTreeView: function(tree, callback) {
            var domTree = $("[data-sc-id='" + tree.attributes.name + "']");
            if (domTree.length == 0)
                return;
            $.each(domTree, function (key, thisTree) {
                var rootNode = $(thisTree).dynatree("getRoot");
                rootNode.visit(callback);
            });
        },
        expandAllTreeViews:function() {
            this.expandTreeView(this.TreeDsTemplate);
            this.expandTreeView(this.TreeDsBaseTemplates);

            window.setTimeout(function () {
                myListPageDialog.expandTreeView(myListPageDialog.TreeDsTemplate);
                myListPageDialog.expandTreeView(myListPageDialog.TreeDsBaseTemplates);
            }, 500);
            window.setTimeout(function () {
                myListPageDialog.expandTreeView(myListPageDialog.TreeDsTemplate);
                myListPageDialog.expandTreeView(myListPageDialog.TreeDsBaseTemplates);
            }, 1000);
        },
        expandTreeView: function (tree) {
            this.visitTreeView(tree, function (node) {
                if (!node.isExpanded()) {
                    if (node.isLazy()) {
                        node.toggleExpand();
                    }
                    else
                        node.expand(true);
                }
            });
        },
        collapseAllTreeViews : function() {
            this.collapseTreeView(this.TreeDsTemplate);
            this.collapseTreeView(this.TreeDsBaseTemplates);
        },
        collapseTreeView: function (tree) {
            this.visitTreeView(tree, function (node) {
                if (node.isExpanded()) {
                    node.expand(false);
                }
            });
        },
        checkAllTreeViews: function () {
            debugger;
            this.checkTreeView(this.TreeDsTemplate);
            this.checkTreeView(this.TreeDsBaseTemplates);
        },
        checkTreeView: function (tree) {
            this.visitTreeView(tree, function (node) {
                if (node.isExpanded()) {
                    node.select(true);
                }
            });
        },
        uncheckAllTreeViews: function () {
            this.uncheckTreeView(this.TreeDsTemplate);
            this.uncheckTreeView(this.TreeDsBaseTemplates);
        },
        uncheckTreeView: function (tree) {
            this.visitTreeView(tree, function (node) {
                if (node.isExpanded()) {
                    node.select(false);
                }
            });
        },
        getSelectedNodes : function() {
            var trees = new Array(this.TreeDsTemplate, this.TreeDsBaseTemplates);
            var selectedNodes = new Array();
            $.each(trees, function (key, thisTree) {
                myListPageDialog.visitTreeView(thisTree, function (node) {
                    if (node.isSelected()) {
                        selectedNodes[selectedNodes.length] = node;
                    }
                });
            });
            return selectedNodes;
        },
        getRenderingId: function () {
            var id = Sitecore.Helpers.url.getQueryParameters(window.location.href)['cid'];
            if (Sitecore.Helpers.id.isId(id)) {
                return id;
            }
            return null;
        },
        download: function (a, b, c) {
            var selectedNodes = this.getSelectedNodes();

            alert(selectedNodes.length);
            debugger;

            var strPaths = "";
            for (var i = 0; i < selectedNodes.length; i++)
                strPaths += selectedNodes[i].data.path + "|";

            // data-sc-id="DownloadForm"
            var form = $("[data-sc-id='DownloadForm']");
            if (form.length === 0)
                return;
            form.attr("method","POST");

            $("<input/>",
            {
                id: "selectedPaths",
                name: "selectedPaths",
                type: "hidden",
                value: strPaths
            }).appendTo(form);
            $("<input/>",
            {
                id: "renderingId",
                name: "renderingId",
                type: "hidden",
                value: this.getRenderingId()
            }).appendTo(form);

            form.submit();
        }
    });
    return ListPageDialog;
});